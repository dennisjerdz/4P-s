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

namespace _4Ps.Controllers
{
    public class SMSController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string short_code = "21583313";

        // GET: SMS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Receive(string access_token, string subscriber_number)
        {
            string subscriber_number_p = "0" + subscriber_number;

            MobileNumber mobile_no = db.MobileNumbers.Include(m => m.Person).FirstOrDefault(n => n.MobileNo == subscriber_number_p);

            if (mobile_no != null)
            {
                mobile_no.Token = access_token;
                db.SaveChanges();

                Sms sms = new Sms(short_code, access_token);

                dynamic response = sms
                    .SetReceiverAddress("+63" + subscriber_number)
                    .SetMessage("Hello, " + mobile_no.Person.GivenName + " from "+mobile_no.Person.Household.Name+" household. Your 4P's inquiry subscription is now verified. You could now inquire through text.")
                    .SendMessage()
                    .GetDynamicResponse();

                Trace.TraceInformation(subscriber_number);
            }
            else
            {
                Trace.TraceInformation("mobile num doesn't exist");
            }

            return null;
        }

        public ActionResult testFeed()
        {
            var signalr = GlobalHost.ConnectionManager.GetHubContext<feedHub>();
            signalr.Clients.All.addmsg("There is a new message.");

            return null;
        }

        public ActionResult Inquiry()
        {
            String data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            JObject result = JObject.Parse(data);

            Trace.TraceInformation(data.ToString());

            string customer_msg = result["inboundSMSMessageList"]["inboundSMSMessage"][0]["message"].ToString();
            string customer_number = result["inboundSMSMessageList"]["inboundSMSMessage"][0]["senderAddress"].ToString();

            // convert globe api format tel:+639 to 09
            string mobile_number = "0" + customer_number.Substring(7);

            //Console.WriteLine(result);
            Trace.TraceInformation(customer_msg+" from "+mobile_number);

            var pm = db.MobileNumbers.Include("Person").FirstOrDefault(m => m.MobileNo == mobile_number);

            var signalr = GlobalHost.ConnectionManager.GetHubContext<feedHub>();
            signalr.Clients.All.addmsg(mobile_number+" "+customer_msg);

            if (pm != null)
            {
                var record_msg = new Message();
                record_msg.Body = customer_msg;
                record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                record_msg.MobileNumberId = pm.MobileNumberId;

                db.Messages.Add(record_msg);
                db.SaveChanges();

                string[] msg = customer_msg.ToLower().Split(' ');

                Trace.TraceInformation(msg[0]);

                if (msg[0] == "hi" || msg[0] == "hello")
                {
                    try
                    {
                        // use sms function
                        SendSMS(mobile_number, "Hello " + pm.Person.getFullName()+".");
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceInformation(ex.Message);
                    }

                    return null;
                }

                if (msg[0] == "compliance")
                {
                    StringBuilder to_send = new StringBuilder("Your household has ");

                    int[] household_members = db.Persons.Where(p => p.HouseholdId == pm.Person.HouseholdId).Select(e=>e.PersonId).ToArray();

                    var attendance_issues = db.AttendanceIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved==false).ToList();
                    var health_issues = db.HealthCheckupIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();
                    var FDS_issues = db.FDSIssues.Where(a => household_members.Contains(a.PersonId) && a.IsResolved == false).ToList();

                    if (attendance_issues.Count() == 0 && health_issues.Count() == 0 && FDS_issues.Count() == 0)
                    {
                        SendSMS(mobile_number, "Your household is fully complying to 4P's conditions.");
                    }else
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
                            if (health_issues.Count() > 0 || (health_issues.Count()==0 && attendance_issues.Count()>0))
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

                        to_send.Append(". Reply 'compliance-health' or 'compliance-attendance' or 'compliance-FDS' for more details.");
                        SendSMS(mobile_number, to_send.ToString());
                    }
                }

            }
            else
            {
                var record_msg = new Message();
                record_msg.Body = customer_msg;
                record_msg.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                record_msg.MobileNumberId = null;

                db.Messages.Add(record_msg);
                db.SaveChangesAsync();
            }

            //return Content(result.ToString(), "application/json");
            return null;
        }

        private ActionResult SendSMS(string mobile_number, string message)
        {
            string access_token = db.MobileNumbers.Where(m => m.MobileNo == mobile_number).FirstOrDefault().Token;

            Sms sms = new Sms(short_code, access_token);

            // mobile number argument is with format 09, convert it to +639
            string globe_format_receiver = "+63" + mobile_number.Substring(1);

            dynamic response = sms.SetReceiverAddress(globe_format_receiver)
                .SetMessage(message)
                .SendMessage()
                .GetDynamicResponse();

            Trace.TraceInformation("Sent a message.");

            return null;
        }
    }
}