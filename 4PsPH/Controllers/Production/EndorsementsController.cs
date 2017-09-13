using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4PsPH.Models;
using Globe.Connect;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using _4PsPH.Hubs;
using _4PsPH.Extensions;

namespace _4PsPH.Controllers
{
    [System.Web.Mvc.Authorize]
    public class EndorsementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /* send sms action */
        //public string short_code = "21584812";
        public string short_code = "21582183";
        private ActionResult SMS(string mobile_number, string message)
        {
            MobileNumber mb = db.MobileNumbers.FirstOrDefault(m => m.MobileNo == mobile_number);
            string access_token = mb.Token;

            if (access_token != null)
            {
                Sms sms = new Sms(short_code, access_token);

                // mobile number argument is with format 09, convert it to +639
                string globe_format_receiver = "+63" + mobile_number.Substring(1);

                dynamic response = sms.SetReceiverAddress(globe_format_receiver)
                    .SetMessage(message)
                    .SendMessage()
                    .GetDynamicResponse();

                Trace.TraceInformation("Sent message; " + message + " to; " + globe_format_receiver + ".");
            }

            return null;
        }
        /* end of send sms action */

        // GET: Endorsements
        public ActionResult Index()
        {
            var endorsements = db.Endorsements.Include(e => e.Ticket);
            return View(endorsements.ToList());
        }

        // GET: Endorsements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement endorsement = db.Endorsements.Find(id);
            if (endorsement == null)
            {
                return HttpNotFound();
            }
            return View(endorsement);
        }

        // GET: Endorsements/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t => t.Person).FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            Endorsement e = new Endorsement();
            e.TicketId = ticket.TicketId;
            e.Ticket = ticket;

            return View(e);
        }

        // POST: Endorsements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EndorsementId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] Endorsement endorsement)
        {
            endorsement.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            endorsement.LastUpdated = DateTime.UtcNow.AddHours(8);
            endorsement.IsApproved = false;

            if (ModelState.IsValid)
            {
                db.Endorsements.Add(endorsement);

                if (db.CaseSummaryReports.Where(e => e.TicketId == endorsement.TicketId).Count() > 0)
                {
                    Ticket ticket = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == endorsement.TicketId);
                    ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Pending OIC Approval").StatusId;

                    if(ticket.MobileNumberId != null)
                    {
                        try
                        {
                            string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " is now pending for OIC approval.";
                            SMS(ticket.MobileNumber.MobileNo, msg);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + e.Message);
                        }
                    }

                    //signalr notify oic
                    var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                    signalr.Clients.Group("OIC" + "-" + User.Identity.GetCityName()).grpmsg("Ticket with ID; " + ticket.TicketId + " is now Pending for OIC Approval.");
                }

                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = endorsement.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", endorsement.TicketId);
            return View(endorsement);
        }

        // GET: Endorsements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement e = db.Endorsements.Include(c => c.Ticket).FirstOrDefault(c => c.EndorsementId == id);
            if (e == null)
            {
                return HttpNotFound();
            }

            return View(e);
        }

        // POST: Endorsements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EndorsementId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] Endorsement endorsement)
        {
            endorsement.LastUpdated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                db.Entry(endorsement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = endorsement.TicketId });
            }

            return View(endorsement);
        }

        // GET: Endorsements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement endorsement = db.Endorsements.Find(id);
            if (endorsement == null)
            {
                return HttpNotFound();
            }
            return View(endorsement);
        }

        // POST: Endorsements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Endorsement endorsement = db.Endorsements.Find(id);
            db.Endorsements.Remove(endorsement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult oic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement e = db.Endorsements.Include(c => c.Ticket).FirstOrDefault(c => c.EndorsementId == id);
            if (e == null)
            {
                return HttpNotFound();
            }

            if (e.IsApproved.Value == false)
            {
                e.IsApproved = true;
                e.DateTimeApproved = DateTime.UtcNow.AddHours(8);

                if (e.Ticket.CaseSummaryReports.Where(c => c.IsApproved == true).Count() > 0)
                {
                    Ticket ticket = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == e.TicketId);
                    ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Waiting for Resolution").StatusId;

                    if (ticket.MobileNumberId !=null)
                    {
                        try
                        {
                            string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " has been approved by the OIC, please wait for resolution.";
                            SMS(ticket.MobileNumber.MobileNo, msg);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + ex.Message);
                        }
                    }

                    //signalr notify oic
                    var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                    signalr.Clients.Group(User.Identity.GetCityName()).grpmsg("Ticket with ID; " + ticket.TicketId + " is now Waiting for Resolution.");
                }
            }
            else
            {
                e.IsApproved = false;
                e.DateTimeApproved = null;

                Ticket ticket = db.Tickets.FirstOrDefault(t => t.TicketId == e.TicketId);
                ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Pending OIC Approval").StatusId;

                //signalr notify oic
                var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                signalr.Clients.Group("Social Worker" + "-" + User.Identity.GetCityName()).grpmsg("OIC has withdrawn approval for Ticket with ID; " + ticket.TicketId + ".");
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Tickets", new { id = e.TicketId });
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
