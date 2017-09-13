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

namespace _4PsPH.Views
{
    [System.Web.Mvc.Authorize]
    public class CaseSummaryReportsController : Controller
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

        // GET: CaseSummaryReports
        public ActionResult Index()
        {
            var caseSummaryReports = db.CaseSummaryReports.Include(c => c.Ticket);
            return View(caseSummaryReports.ToList());
        }

        // GET: CaseSummaryReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReport caseSummaryReport = db.CaseSummaryReports.Find(id);
            if (caseSummaryReport == null)
            {
                return HttpNotFound();
            }
            return View(caseSummaryReport);
        }

        // GET: CaseSummaryReports/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t=>t.Person).FirstOrDefault(t=>t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            CaseSummaryReport csr = new CaseSummaryReport();
            csr.TicketId = ticket.TicketId;
            csr.Ticket = ticket;

            return View(csr);
        }

        // POST: CaseSummaryReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CaseSummaryReportId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] CaseSummaryReport caseSummaryReport)
        {
            caseSummaryReport.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            caseSummaryReport.LastUpdated = DateTime.UtcNow.AddHours(8);
            caseSummaryReport.IsApproved = false;

            if (ModelState.IsValid)
            {
                db.CaseSummaryReports.Add(caseSummaryReport);

                if(db.Endorsements.Where(e=>e.TicketId == caseSummaryReport.TicketId).Count() > 0)
                {
                    Ticket ticket = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == caseSummaryReport.TicketId);
                    ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Pending OIC Approval").StatusId;

                    if (ticket.MobileNumberId !=null)
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
                return RedirectToAction("Details","Tickets", new { id= caseSummaryReport.TicketId });
            }

            return View(caseSummaryReport);
        }

        // GET: CaseSummaryReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReport caseSummaryReport = db.CaseSummaryReports.Include(c=>c.Ticket).FirstOrDefault(c=>c.CaseSummaryReportId == id);
            if (caseSummaryReport == null)
            {
                return HttpNotFound();
            }

            return View(caseSummaryReport);
        }

        // POST: CaseSummaryReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CaseSummaryReportId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] CaseSummaryReport caseSummaryReport)
        {
            caseSummaryReport.LastUpdated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                db.Entry(caseSummaryReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","Tickets", new { id=caseSummaryReport.TicketId });
            }

            return View(caseSummaryReport);
        }

        // GET: CaseSummaryReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReport caseSummaryReport = db.CaseSummaryReports.Find(id);
            if (caseSummaryReport == null)
            {
                return HttpNotFound();
            }
            return View(caseSummaryReport);
        }

        // POST: CaseSummaryReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CaseSummaryReport caseSummaryReport = db.CaseSummaryReports.Find(id);
            db.CaseSummaryReports.Remove(caseSummaryReport);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult oic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReport csr = db.CaseSummaryReports.Include(c => c.Ticket).FirstOrDefault(c => c.CaseSummaryReportId == id);
            if (csr == null)
            {
                return HttpNotFound();
            }

            if (csr.IsApproved.Value == false)
            {
                csr.IsApproved = true;
                csr.DateTimeApproved = DateTime.UtcNow.AddHours(8);

                if (csr.Ticket.Endorsements.Where(e => e.IsApproved == true).Count() > 0)
                {
                    Ticket ticket = db.Tickets.Include("Person").Include("Category").Include("MobileNumber").FirstOrDefault(t => t.TicketId == csr.TicketId);
                    ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Waiting for Resolution").StatusId;

                    if (ticket.MobileNumberId !=null)
                    {
                        try
                        {
                            string msg = "Hello " + ticket.Person.GivenName + ", the " + ticket.Category.Name + " ticket with ID; " + ticket.TicketId + " has been approved by the OIC, please wait for resolution.";
                            SMS(ticket.MobileNumber.MobileNo, msg);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceInformation("Unable to send message to " + ticket.MobileNumber.MobileNo + " with error; " + e.Message);
                        }
                    }

                    //signalr notify oic
                    var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                    signalr.Clients.Group(User.Identity.GetCityName()).grpmsg("Ticket with ID; " + ticket.TicketId + " is now Waiting for Resolution.");
                }
            }
            else
            {
                csr.IsApproved = false;
                csr.DateTimeApproved = null;

                Ticket ticket = db.Tickets.FirstOrDefault(t => t.TicketId == csr.TicketId);
                ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Pending OIC Approval").StatusId;

                //signalr notify oic
                var signalr = GlobalHost.ConnectionManager.GetHubContext<FeedHub>();
                signalr.Clients.Group("Social Worker" + "-" + User.Identity.GetCityName()).grpmsg("OIC has withdrawn approval for Ticket with ID; " + ticket.TicketId + ".");
            }

            db.SaveChanges();

            return RedirectToAction("Details","Tickets", new { id=csr.TicketId });
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
