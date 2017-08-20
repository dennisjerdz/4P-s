using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4PsPH.Models;
using _4PsPH.Extensions;
using Microsoft.AspNet.Identity;
using Globe.Connect;
using System.Diagnostics;

namespace _4PsPH.Controllers.Production
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /* send sms action */
        public string short_code = "21583313";
        private ActionResult SMS(string mobile_number, string message)
        {
            MobileNumber mb = db.MobileNumbers.FirstOrDefault(m => m.MobileNo == mobile_number);
            string access_token = mb.Token;

            if(access_token != null)
            {
                Sms sms = new Sms(short_code, access_token);

                // mobile number argument is with format 09, convert it to +639
                string globe_format_receiver = "+63" + mobile_number.Substring(1);

                dynamic response = sms.SetReceiverAddress(globe_format_receiver)
                    .SetMessage(message)
                    .SendMessage()
                    .GetDynamicResponse();

                Trace.TraceInformation("Sent message; " + message + " to; "+globe_format_receiver+".");
            }

            return null;
        }
        /* end of send sms action */

        public ActionResult Resolve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t => t.Person).Include(t=>t.Category).Include(t=>t.MobileNumber).FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(ticket);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Resolve([Bind(Include = "TicketId,ActionAdvised,PersonId,CategoryId,StatusId,MobileNumberId,DateTimeCreated,IdAttached,Comment,CreatedAtOffice")] Ticket ticket)
        {
            ticket.ResolvedBy = User.Identity.GetFullName();
            ticket.ResolvedByUsername = User.Identity.Name;
            ticket.ResolvedDate = DateTime.UtcNow.AddHours(8);
            ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Resolved").StatusId;

            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                Ticket nt = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == ticket.TicketId);

                try
                {
                    string msg = "Hello " + nt.Person.GivenName + ", the "+nt.Category.Name+" ticket with ID; " + nt.TicketId + " has been resolved; action advised is '"+ticket.ActionAdvised+"'.";
                    SMS(nt.MobileNumber.MobileNo,msg);
                }catch(Exception e)
                {
                    Trace.TraceInformation("Unable to send message to "+nt.MobileNumber.MobileNo+" with error; "+e.Message);
                }

                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: Tickets
        public ActionResult Index()
        {
            int city = Convert.ToInt16(User.Identity.GetCityId());

            var tickets = db.Tickets.Include(t => t.Category).Include(t => t.MobileNumber).Include(t => t.Person).Include(t => t.Status).Where(t=>t.Person.Household.CityId == city);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t=>t.TicketComments).FirstOrDefault(t=>t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            TicketComment tc = new TicketComment();
            tc.TicketId = ticket.TicketId;
            tc.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            tc.Ticket = ticket;

            return View(tc);
        }

        // GET: Tickets/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person p = db.Persons.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            Ticket t = new Ticket();
            t.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            t.PersonId = p.PersonId;
            t.Person = p;

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == p.HouseholdId).Select(e=>e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View(t);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,PersonId,CategoryId,MobileNumberId,DateTimeCreated,IdAttached,Comment")] Ticket ticket)
        {
            ticket.CreatedAtOffice = true;
            ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Waiting for Verification").StatusId;

            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();

                Ticket nt = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == ticket.TicketId);

                if (nt.MobileNumberId != null)
                {
                    try
                    {
                        string msg = "Hello " + nt.Person.GivenName + ", a ticket of " + nt.Category.Name + " category with ID; " + nt.TicketId + " has been created.";
                        SMS(nt.MobileNumber.MobileNo, msg);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation("Unable to send message to " + nt.MobileNumber.MobileNo + " with error; " + e.Message);
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketId,PersonId,CategoryId,StatusId,MobileNumberId,DateTimeCreated,IdAttached,Comment,CreatedAtOffice")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "TicketCommentId,TicketId,Body,DateTimeCreated")] TicketComment ticketComment)
        {
            ticketComment.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            ticketComment.Ticket = db.Tickets.Include(t => t.TicketComments).FirstOrDefault(t => t.TicketId == ticketComment.TicketId);
            ticketComment.CreatedBy = User.Identity.GetFullName();
            ticketComment.CreatedByUsername = User.Identity.Name;

            string type = "";
            if (User.IsInRole("4P's Officer"))
            {
                type = "4P's Officer";
            }
            if (User.IsInRole("Social Worker"))
            {
                type = "Social Worker";
            }
            if (User.IsInRole("OIC"))
            {
                type = "OIC";
            }

            ticketComment.CreatedByType = type;

            if (ModelState.IsValid)
            {
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            return View(ticketComment);
        }

        public ActionResult Next(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t=>t.Person).Include(t=>t.Status).Include(t=>t.Category).Include(t => t.MobileNumber).FirstOrDefault(t=>t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            string current_status = ticket.Status.Name;

            if (current_status == "Waiting for Verification")
            {
                ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Verified / Pending Endorsement" ).StatusId;

                if(ticket.MobileNumberId != null)
                {
                    try
                    {
                        string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " has been verified.";
                        SMS(ticket.MobileNumber.MobileNo, msg);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + e.Message);
                    }
                }
            }
            else if(current_status == "Verified / Pending Endorsement")
            {

            }
            else if(current_status == "Pending OIC Approval")
            {

            }else if(current_status == "Waiting for Resolution")
            {

            }else if(current_status == "Resolved")
            {

            }else
            {

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Revert(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include("Status").Include(t=>t.Category).Include(t=>t.MobileNumber).Include("CaseSummaryReports").Include("Endorsements").FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            string current_status = ticket.Status.Name;

            if (current_status == "Waiting for Verification")
            {

            }
            else if (current_status == "Verified / Pending Endorsement")
            {
                ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Waiting for Verification").StatusId;

                if(ticket.MobileNumberId !=null)
                {
                    try
                    {
                        string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " has been reverted to waiting for verification.";
                        SMS(ticket.MobileNumber.MobileNo, msg);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + e.Message);
                    }
                }
            }
            else if (current_status == "Pending OIC Approval")
            {
                
            }
            else if (current_status == "Waiting for Resolution")
            {
                if (User.IsInRole("OIC"))
                {
                    ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Pending OIC Approval").StatusId;

                    ticket.Endorsements.First().DateTimeApproved = null;
                    ticket.Endorsements.First().IsApproved = false;

                    ticket.CaseSummaryReports.First().DateTimeApproved = null;
                    ticket.CaseSummaryReports.First().IsApproved = false;

                    if (ticket.MobileNumberId !=null)
                    {
                        try
                        {
                            string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " has been reverted to pending OIC approval.";
                            SMS(ticket.MobileNumber.MobileNo, msg);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + e.Message);
                        }
                    }
                }
            }
            else if (current_status == "Resolved")
            {

            }
            else
            {

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
