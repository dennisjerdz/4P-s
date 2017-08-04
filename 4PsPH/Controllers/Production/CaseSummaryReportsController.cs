using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4PsPH.Models;

namespace _4PsPH.Controllers
{
    public class CaseSummaryReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached");
            return View();
        }

        // POST: CaseSummaryReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CaseSummaryReportId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] CaseSummaryReport caseSummaryReport)
        {
            if (ModelState.IsValid)
            {
                db.CaseSummaryReports.Add(caseSummaryReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", caseSummaryReport.TicketId);
            return View(caseSummaryReport);
        }

        // GET: CaseSummaryReports/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", caseSummaryReport.TicketId);
            return View(caseSummaryReport);
        }

        // POST: CaseSummaryReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CaseSummaryReportId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] CaseSummaryReport caseSummaryReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caseSummaryReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", caseSummaryReport.TicketId);
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
