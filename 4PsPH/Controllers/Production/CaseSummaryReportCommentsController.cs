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
    public class CaseSummaryReportCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CaseSummaryReportComments
        public ActionResult Index()
        {
            var caseSummaryReportComment = db.CaseSummaryReportComment.Include(c => c.CaseSummaryReport);
            return View(caseSummaryReportComment.ToList());
        }

        // GET: CaseSummaryReportComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReportComment caseSummaryReportComment = db.CaseSummaryReportComment.Find(id);
            if (caseSummaryReportComment == null)
            {
                return HttpNotFound();
            }
            return View(caseSummaryReportComment);
        }

        // GET: CaseSummaryReportComments/Create
        public ActionResult Create()
        {
            ViewBag.CaseSummaryReportId = new SelectList(db.CaseSummaryReports, "CaseSummaryReportId", "CreatedBy");
            return View();
        }

        // POST: CaseSummaryReportComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CaseSummaryReportCommentId,CaseSummaryReportId,Body,DateTimeCreated")] CaseSummaryReportComment caseSummaryReportComment)
        {
            if (ModelState.IsValid)
            {
                db.CaseSummaryReportComment.Add(caseSummaryReportComment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CaseSummaryReportId = new SelectList(db.CaseSummaryReports, "CaseSummaryReportId", "CreatedBy", caseSummaryReportComment.CaseSummaryReportId);
            return View(caseSummaryReportComment);
        }

        // GET: CaseSummaryReportComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReportComment caseSummaryReportComment = db.CaseSummaryReportComment.Find(id);
            if (caseSummaryReportComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CaseSummaryReportId = new SelectList(db.CaseSummaryReports, "CaseSummaryReportId", "CreatedBy", caseSummaryReportComment.CaseSummaryReportId);
            return View(caseSummaryReportComment);
        }

        // POST: CaseSummaryReportComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CaseSummaryReportCommentId,CaseSummaryReportId,Body,DateTimeCreated")] CaseSummaryReportComment caseSummaryReportComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caseSummaryReportComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CaseSummaryReportId = new SelectList(db.CaseSummaryReports, "CaseSummaryReportId", "CreatedBy", caseSummaryReportComment.CaseSummaryReportId);
            return View(caseSummaryReportComment);
        }

        // GET: CaseSummaryReportComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseSummaryReportComment caseSummaryReportComment = db.CaseSummaryReportComment.Find(id);
            if (caseSummaryReportComment == null)
            {
                return HttpNotFound();
            }
            return View(caseSummaryReportComment);
        }

        // POST: CaseSummaryReportComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CaseSummaryReportComment caseSummaryReportComment = db.CaseSummaryReportComment.Find(id);
            db.CaseSummaryReportComment.Remove(caseSummaryReportComment);
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
