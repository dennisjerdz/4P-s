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
    public class FDSIssuesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FDSIssues
        public ActionResult Index()
        {
            var fDSIssues = db.FDSIssues.Include(f => f.FDS).Include(f => f.Person);
            return View(fDSIssues.ToList());
        }

        // GET: FDSIssues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDSIssue fDSIssue = db.FDSIssues.Find(id);
            if (fDSIssue == null)
            {
                return HttpNotFound();
            }
            return View(fDSIssue);
        }

        // GET: FDSIssues/Create
        public ActionResult Create()
        {
            ViewBag.FDSId = new SelectList(db.FDS, "FDSId", "Name");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            return View();
        }

        // POST: FDSIssues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FDSIssueId,Comment,IsResolved,ResolvedDate,DateTimeCreated,FDSId,PersonId")] FDSIssue fDSIssue)
        {
            if (ModelState.IsValid)
            {
                db.FDSIssues.Add(fDSIssue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FDSId = new SelectList(db.FDS, "FDSId", "Name", fDSIssue.FDSId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", fDSIssue.PersonId);
            return View(fDSIssue);
        }

        // GET: FDSIssues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDSIssue fDSIssue = db.FDSIssues.Find(id);
            if (fDSIssue == null)
            {
                return HttpNotFound();
            }
            ViewBag.FDSId = new SelectList(db.FDS, "FDSId", "Name", fDSIssue.FDSId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", fDSIssue.PersonId);
            return View(fDSIssue);
        }

        // POST: FDSIssues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FDSIssueId,Comment,IsResolved,ResolvedDate,DateTimeCreated,FDSId,PersonId")] FDSIssue fDSIssue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fDSIssue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FDSId = new SelectList(db.FDS, "FDSId", "Name", fDSIssue.FDSId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", fDSIssue.PersonId);
            return View(fDSIssue);
        }

        // GET: FDSIssues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDSIssue fDSIssue = db.FDSIssues.Find(id);
            if (fDSIssue == null)
            {
                return HttpNotFound();
            }
            return View(fDSIssue);
        }

        // POST: FDSIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FDSIssue fDSIssue = db.FDSIssues.Find(id);
            db.FDSIssues.Remove(fDSIssue);
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
