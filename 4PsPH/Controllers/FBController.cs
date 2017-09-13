using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Globe.Connect;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using _4PsPH.Models;
using System.Text;
using Microsoft.AspNet.SignalR;
using _4PsPH.Hubs;
using _4PsPH.Extensions;
using System.Globalization;
using System.Net;

namespace _4PsPH.Controllers
{
    public class FBController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public string short_code = "21584812";
        public string short_code = "21582183";

        // GET: FB
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult testFeed(string msg, string city)
        {
            var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
            signalr.Clients.Group(city).grpmsg(msg);

            return null;
        }

        private ActionResult SendSMS(string mobile_number, string message)
        {
            MobileNumber mb = db.MobileNumbers.FirstOrDefault(m => m.MobileNo == mobile_number);
            string access_token = mb.Token;

            if (access_token != null)
            {
                try
                {
                    Sms sms = new Sms(short_code, access_token);

                    // mobile number argument is with format 09, convert it to +639
                    string globe_format_receiver = "+63" + mobile_number.Substring(1);

                    dynamic response = sms.SetReceiverAddress(globe_format_receiver)
                        .SetMessage(message)
                        .SendMessage()
                        .GetDynamicResponse();

                    Trace.TraceInformation("Sent message; " + message);
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Unable to send message to " + mobile_number + ". Error; " + e.Message);
                }
            }

            return null;
        }

        public ActionResult GetFBMsg(string content)
        {
            Trace.TraceInformation("MSG is "+content);

            string[] split = content.Split(' ');

            string mobile_number = split[0];

            if (mobile_number.All(char.IsDigit))
            {
                var pm = db.MobileNumbers.Include("Person").FirstOrDefault(m => m.MobileNo == mobile_number);

                if (pm != null && pm.IsDisabled == false)
                {
                    string customer_msg = string.Join(" ", split.Skip(1));
                    string[] msg = string.Join(" ", split.Skip(1)).Split(' ');

                    Trace.TraceInformation(msg[0]);

                    #region hello
                    if (msg[0] == "hi" || msg[0] == "hello")
                    {
                        try
                        {
                            // use sms function
                            SendSMS(mobile_number, "Hello " + pm.Person.getFullName() + ".");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation(ex.Message);
                        }

                        //return new HttpStatusCodeResult(HttpStatusCode.OK, "Hello " + pm.Person.getFullName() + ".");
                        return Content("Hello " + pm.Person.getFullName() + ".");
                    }
                    #endregion

                    #region learn more
                    if ((msg[0] == "learn" && msg[1] == "more") || msg[0] == "tulong" || msg[0] == "saklolo")
                    {
                        try
                        {
                            // use sms function
                            SendSMS(mobile_number, "List of sms keywords: 'hello' - System will send an sms with your first name, 'compliance' - Send a summary of all issues associated with household if there are any, 'tickets' - Send a list of all tickets associated with household, '<Ticket ID> <message>' - Post a comment to the specific ticket ID that the social workers could view, 'Ticket <Ticket ID>' -  Show specific ticket details if exists, '? <message>' - Ask a specific question that a Social Worker can respond to.");
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation(ex.Message);
                        }

                        #region record msg
                        var record_msg = new Message();
                        record_msg.Body = customer_msg;
                        record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                        record_msg.MobileNumberId = pm.MobileNumberId;

                        db.Messages.Add(record_msg);
                        db.SaveChanges();
                        #endregion

                        return Content("List of sms keywords: 'hello' - System will send an sms with your first name, 'compliance' - Send a summary of all issues associated with household if there are any, 'tickets' - Send a list of all tickets associated with household, '<Ticket ID> <message>' - Post a comment to the specific ticket ID that the social workers could view, 'Ticket <Ticket ID>' -  Show specific ticket details if exists, '? <message>' - Ask a specific question that a Social Worker can respond to.");
                    }
                    #endregion

                    #region msg ticket
                    if (msg[0].All(char.IsDigit))
                    {
                        int ticket_id = Convert.ToInt16(msg[0]);
                        if (ticket_id == 0)
                        {
                            return null;
                        }
                        else
                        {
                            Ticket ticket = db.Tickets.Include(t => t.Person).Include(t => t.Person.MobileNumbers).Include(t => t.Person.Household).FirstOrDefault(t => t.TicketId == ticket_id);

                            if (ticket == null)
                            {
                                SendSMS(mobile_number, "Sorry, the ticket ID you requested cannot be found.");

                                return Content("Sorry, the ticket ID you requested cannot be found.");
                            }
                            else
                            {
                                if (ticket.Person.Household.People.Any(m => m.MobileNumbers.Any(o => o.MobileNo == mobile_number)))
                                {
                                    string comment = string.Join(" ", msg.Skip(1));

                                    TicketComment tc = new TicketComment();
                                    tc.Body = comment;
                                    tc.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                                    tc.CreatedBy = pm.Person.getFullName();
                                    tc.CreatedByType = "client";
                                    tc.CreatedByUsername = pm.PersonId.ToString();
                                    tc.TicketId = ticket.TicketId;

                                    db.TicketComments.Add(tc);

                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.TraceInformation("Failed to add comment to Ticket ID " + ticket.TicketId + " from " + mobile_number + " with error; " + ex.Message);
                                        return null;
                                    }

                                    SendSMS(mobile_number, "Message to Ticket ID " + ticket.TicketId + " has been created.");
                                    #region record msg
                                    var record_msg = new Message();
                                    record_msg.Body = customer_msg;
                                    record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                                    record_msg.MobileNumberId = pm.MobileNumberId;

                                    db.Messages.Add(record_msg);
                                    db.SaveChanges();
                                    #endregion

                                    /*signalr notification*/
                                    var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                                    signalr.Clients.Group(pm.Person.Household.City.Name).grpmsg(pm.Person.GivenName + ": " + customer_msg);

                                    return Content("Message to Ticket ID " + ticket.TicketId + " has been created.");
                                }
                            }
                        }
                    }
                    #endregion

                    #region create ticket
                    if (msg[0] == "ticket" && msg[1] == "create")
                    {
                        if (msg[2] == "payment" || msg[2] == "complianceverification" || msg[2] == "others")
                        {
                            string c_received = "";
                            if (msg[2] == "complianceverification")
                            {
                                c_received = "compliance verification";
                            }
                            else
                            {
                                c_received = msg[2].ToLower();
                            }

                            Ticket t = new Ticket();
                            t.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            t.CategoryId = db.Categories.FirstOrDefault(c => c.Name.ToLower() == c_received).CategoryId;
                            t.CreatedAtOffice = false;
                            t.IdAttached = null;
                            t.MobileNumberId = pm.MobileNumberId;
                            t.PersonId = pm.PersonId;
                            t.StatusId = db.Statuses.FirstOrDefault(c => c.Name == "Waiting for Verification").StatusId;

                            db.Tickets.Add(t);

                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation("Failed to create ticket from " + mobile_number + " with error; " + ex.Message);
                                return null;
                            }

                            SendSMS(mobile_number, "Ticket ID " + t.TicketId + " with category; " + c_received + " has been created.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Ticket ID " + t.TicketId + " with category; " + c_received + " has been created.");
                        }
                        else
                        {
                            SendSMS(mobile_number, "Sorry, these are the only categories supported; 'Payment', 'ComplianceVerification', 'Others'.");
                            return Content("Sorry, these are the only categories supported; 'Payment', 'ComplianceVerification', 'Others'.");
                        }
                    }
                    #endregion

                    #region ticket <ID>
                    if (msg[0] == "ticket")
                    {
                        int s = Convert.ToInt16(msg[1]);

                        Ticket ticket = db.Tickets.Include(t => t.Category).Include(t => t.Person).Include(t => t.Status).FirstOrDefault(t => t.TicketId == s);

                        if (ticket == null)
                        {
                            SendSMS(mobile_number, "The ticket doesn't exist.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("The ticket doesn't exist.");
                        }
                        else
                        {
                            SendSMS(mobile_number, "Ticket ID " + ticket.TicketId + ", status: " + ticket.Status.Name + ", category: " + ticket.Category.Name + ", and comment: " + ticket.Comment + ticket.ActionAdvised + ".");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Ticket ID " + ticket.TicketId + ", status: " + ticket.Status.Name + ", category: " + ticket.Category.Name + ", and comment: " + ticket.Comment + ticket.ActionAdvised + ".");
                        }
                    }
                    #endregion

                    #region tickets
                    if (msg[0] == "tickets")
                    {
                        int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e => e.PersonId).ToArray();

                        var tickets = db.Tickets.Include("Category").Include("Status").Where(t => household_members.Contains(t.PersonId));

                        StringBuilder to_send = new StringBuilder("Your household, " + pm.Person.Household.Name + ", has ");

                        if (tickets.Count() == 0)
                        {
                            SendSMS(mobile_number, "Your household, " + pm.Person.Household.Name + ", doesn't have any tickets.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Your household, " + pm.Person.Household.Name + ", doesn't have any tickets.");
                        }
                        else
                        {
                            var unresolved = tickets.Where(w => w.Status.Name != "Resolved");
                            var resolved = tickets.Where(w => w.Status.Name == "Resolved");

                            if (resolved.Count() != 0 && unresolved.Count() != 0)
                            {
                                to_send.Append(resolved.Count() + "resolved tickets and the following unresolved; ");
                                foreach (var x in unresolved)
                                {
                                    to_send.Append(x.Category.Name + " Ticket w/ ID " + x.TicketId + " - " + x.Status.Name + ".");
                                }
                            }

                            if (unresolved.Count() != 0 && resolved.Count() == 0)
                            {
                                to_send.Append("the following unresolved tickets; ");
                                foreach (var x in unresolved)
                                {
                                    to_send.Append(x.Category.Name + " Ticket w/ ID " + x.TicketId + " - " + x.Status.Name + ".");
                                }
                            }

                            if (unresolved.Count() == 0 && resolved.Count() != 0)
                            {
                                to_send.Append(resolved.Count() + " resolved tickets.");
                                to_send.Append(" Reply 'ticket <ID>' for more details.");
                            }

                            SendSMS(mobile_number, to_send.ToString());

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content(to_send.ToString());
                        }
                    }
                    #endregion

                    #region health compliance
                    if (msg[0] == "compliance-health")
                    {
                        StringBuilder to_send = new StringBuilder("Unresolved Health Compliance Issues: ");

                        int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e => e.PersonId).ToArray();
                        var health_issues = db.HealthCheckupIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();

                        if (health_issues.Count == 0)
                        {
                            SendSMS(mobile_number, "Your household, " + pm.Person.Household.Name + ", doesn't have any health-related compliance issues.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Your household, " + pm.Person.Household.Name + ", doesn't have any health-related compliance issues.");
                        }
                        else
                        {
                            foreach (var x in health_issues)
                            {
                                to_send.Append("Issue ID " + x.HealthCheckupIssueId + "-" + x.Comment);
                                to_send.Append(" ");
                            }

                            SendSMS(mobile_number, to_send.ToString());

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content(to_send.ToString());
                        }
                    }
                    #endregion

                    #region school compliance
                    if (msg[0] == "compliance-school")
                    {
                        StringBuilder to_send = new StringBuilder("");

                        int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e => e.PersonId).ToArray();
                        var attendance_issues = db.AttendanceIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();

                        if (attendance_issues.Count == 0)
                        {
                            SendSMS(mobile_number, "Your household, " + pm.Person.Household.Name + ", doesn't have any school-related attendance issues.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Your household, " + pm.Person.Household.Name + ", doesn't have any school-related attendance issues.");
                        }
                        else
                        {
                            foreach (var x in attendance_issues)
                            {
                                to_send.Append("Issue ID " + x.AttendanceIssueId + "-" + x.Comment);
                                to_send.Append(" ");
                            }

                            SendSMS(mobile_number, to_send.ToString());

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content(to_send.ToString());
                        }
                    }
                    #endregion

                    #region fds compliance
                    if (msg[0] == "compliance-fds")
                    {
                        StringBuilder to_send = new StringBuilder("Unresolved FDS Issues: ");

                        int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e => e.PersonId).ToArray();
                        var FDS_issues = db.FDSIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();

                        if (FDS_issues.Count == 0)
                        {
                            SendSMS(mobile_number, "Your household, " + pm.Person.Household.Name + ", doesn't have any Family development attendance issues.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Your household, " + pm.Person.Household.Name + ", doesn't have any Family development attendance issues.");
                        }
                        else
                        {
                            foreach (var x in FDS_issues)
                            {
                                to_send.Append("Issue ID " + x.FDSIssueId + "-" + x.Comment);
                                to_send.Append(" ");
                            }

                            SendSMS(mobile_number, to_send.ToString());

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content(to_send.ToString());
                        }
                    }
                    #endregion

                    #region compliance
                    if (msg[0] == "compliance")
                    {
                        StringBuilder to_send = new StringBuilder("Your household, " + pm.Person.Household.Name + ", has ");

                        int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e => e.PersonId).ToArray();

                        var attendance_issues = db.AttendanceIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();
                        var health_issues = db.HealthCheckupIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();
                        var FDS_issues = db.FDSIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();

                        if (attendance_issues.Count() == 0 && health_issues.Count() == 0 && FDS_issues.Count() == 0)
                        {
                            SendSMS(mobile_number, "Your household, " + pm.Person.Household.Name + ",  is fully complying to 4P's conditions.");

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content("Your household, " + pm.Person.Household.Name + ",  is fully complying to 4P's conditions.");
                        }
                        else
                        {
                            if (attendance_issues.Count() > 0)
                            {
                                if (attendance_issues.Count() == 1)
                                {
                                    to_send.Append(attendance_issues.Count() + " attendance issue");
                                }
                                else
                                {
                                    to_send.Append(attendance_issues.Count() + " attendance issues");
                                }
                            }

                            if (health_issues.Count() > 0)
                            {
                                if (attendance_issues.Count() > 0)
                                {
                                    to_send.Append(", ");
                                }

                                if (health_issues.Count() == 1)
                                {
                                    to_send.Append(health_issues.Count() + " medical issue");
                                }
                                else
                                {
                                    to_send.Append(health_issues.Count() + " medical issues");
                                }
                            }

                            if (FDS_issues.Count() > 0)
                            {
                                if (health_issues.Count() > 0 || (health_issues.Count() == 0 && attendance_issues.Count() > 0))
                                {
                                    to_send.Append(", ");
                                }

                                if (FDS_issues.Count() == 1)
                                {
                                    to_send.Append(FDS_issues.Count() + " FDS issue");
                                }
                                else
                                {
                                    to_send.Append(FDS_issues.Count() + " FDS issues");
                                }
                            }

                            to_send.Append(". Reply 'compliance-health' or 'compliance-school' or 'compliance-FDS' for more details.");
                            SendSMS(mobile_number, to_send.ToString());

                            #region record msg
                            var record_msg = new Message();
                            record_msg.Body = customer_msg;
                            record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                            record_msg.MobileNumberId = pm.MobileNumberId;

                            db.Messages.Add(record_msg);
                            db.SaveChanges();
                            #endregion

                            return Content(to_send.ToString());
                        }
                    }
                    #endregion

                    #region ? create ticket
                    if (msg[0] == "?")
                    {
                        var rec_msg = new Message();
                        rec_msg.Body = customer_msg;
                        rec_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                        rec_msg.MobileNumberId = null;

                        db.Messages.Add(rec_msg);
                        db.SaveChangesAsync();

                        SendSMS(mobile_number, "Your message has been received. Please wait for a message or ticket as a response.");

                        /*signalr notification*/
                        var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                        signalr.Clients.Group(pm.Person.Household.City.Name).grpmsg(pm.Person.GivenName + " - " + mobile_number + ": " + customer_msg);
                    }
                    #endregion

                    return Content("Your message has been received. Please wait for a message or ticket as a response.");
                }
                else
                {
                    return Content("Mobile number incorrect.");
                }
            }else
            {
                return Content("Invalid keyword. Send 'inquire <mobile number> learn more' for the list of available keywords.");
            }
        }
    }
}