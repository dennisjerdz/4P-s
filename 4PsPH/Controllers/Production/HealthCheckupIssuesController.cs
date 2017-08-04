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
    public class HealthCheckupIssuesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HealthCheckupIssues
        public ActionResult Index()
        {
            var healthCheckupIssues = db.HealthCheckupIssues.Include(h => h.Person);
            return View(healthCheckupIssues.ToList());
        }

        // GET: HealthCheckupIssues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCheckupIssue healthCheckupIssue = db.HealthCheckupIssues.Find(id);
            if (healthCheckupIssue == null)
            {
                return HttpNotFound();
            }
            return View(healthCheckupIssue);
        }

        // GET: HealthCheckupIssues/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            return View();
        }

        // POST: HealthCheckupIssues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HealthCheckupIssueId,PersonId,IsResolved,ResolvedDate,Comment")] HealthCheckupIssue healthCheckupIssue)
        {
            if (ModelState.IsValid)
            {
                db.HealthCheckupIssues.Add(healthCheckupIssue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", healthCheckupIssue.PersonId);
            return View(healthCheckupIssue);
        }

        // GET: HealthCheckupIssues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCheckupIssue healthCheckupIssue = db.HealthCheckupIssues.Find(id);
            if (healthCheckupIssue == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", healthCheckupIssue.PersonId);
            return View(healthCheckupIssue);
        }

        // POST: HealthCheckupIssues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HealthCheckupIssueId,PersonId,IsResolved,ResolvedDate,Comment")] HealthCheckupIssue healthCheckupIssue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(healthCheckupIssue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", healthCheckupIssue.PersonId);
            return View(healthCheckupIssue);
        }

        // GET: HealthCheckupIssues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCheckupIssue healthCheckupIssue = db.HealthCheckupIssues.Find(id);
            if (healthCheckupIssue == null)
            {
                return HttpNotFound();
            }
            return View(healthCheckupIssue);
        }

        // POST: HealthCheckupIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HealthCheckupIssue healthCheckupIssue = db.HealthCheckupIssues.Find(id);
            db.HealthCheckupIssues.Remove(healthCheckupIssue);
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
