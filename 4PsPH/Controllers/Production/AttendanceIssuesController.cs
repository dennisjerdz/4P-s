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
    public class AttendanceIssuesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AttendanceIssues
        public ActionResult Index()
        {
            var attendanceIssues = db.AttendanceIssues.Include(a => a.Person).Include(a => a.School);
            return View(attendanceIssues.ToList());
        }

        // GET: AttendanceIssues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceIssue attendanceIssue = db.AttendanceIssues.Find(id);
            if (attendanceIssue == null)
            {
                return HttpNotFound();
            }
            return View(attendanceIssue);
        }

        // GET: AttendanceIssues/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name");
            return View();
        }

        // POST: AttendanceIssues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AttendanceIssueId,PersonId,SchoolId,IsResolved,ResolvedDate,Comment")] AttendanceIssue attendanceIssue)
        {
            if (ModelState.IsValid)
            {
                db.AttendanceIssues.Add(attendanceIssue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", attendanceIssue.PersonId);
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name", attendanceIssue.SchoolId);
            return View(attendanceIssue);
        }

        // GET: AttendanceIssues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceIssue attendanceIssue = db.AttendanceIssues.Find(id);
            if (attendanceIssue == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", attendanceIssue.PersonId);
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name", attendanceIssue.SchoolId);
            return View(attendanceIssue);
        }

        // POST: AttendanceIssues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AttendanceIssueId,PersonId,SchoolId,IsResolved,ResolvedDate,Comment")] AttendanceIssue attendanceIssue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendanceIssue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", attendanceIssue.PersonId);
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name", attendanceIssue.SchoolId);
            return View(attendanceIssue);
        }

        // GET: AttendanceIssues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttendanceIssue attendanceIssue = db.AttendanceIssues.Find(id);
            if (attendanceIssue == null)
            {
                return HttpNotFound();
            }
            return View(attendanceIssue);
        }

        // POST: AttendanceIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttendanceIssue attendanceIssue = db.AttendanceIssues.Find(id);
            db.AttendanceIssues.Remove(attendanceIssue);
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
