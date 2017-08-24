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

namespace _4PsPH.Controllers.Groups
{
    [Authorize]
    public class SchoolsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult GenerateAttendanceForm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Include(s=>s.People).Include(s=>s.City).FirstOrDefault(s=>s.SchoolId == id);
            if (school == null)
            {
                return HttpNotFound();
            }

            return View(school);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateAttendanceForm([Bind(Include = "AttendanceIssues")] School school)
        {
            if(school.AttendanceIssues != null)
            {
                foreach (var x in school.AttendanceIssues)
                {
                    AttendanceIssue ai = new AttendanceIssue();
                    ai.ResolvedDate = null;
                    ai.IsResolved = false;
                    string comment = "The beneficiary didn't attend for the month of " + x.Comment + ".";
                    ai.Comment = comment;
                    ai.PersonId = x.PersonId;
                    ai.SchoolId = x.SchoolId;
                    ai.ResolveComment = null;
                    ai.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.AttendanceIssues.Add(ai);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "AttendanceIssues", null);
            }else
            {
                return RedirectToAction("Index", "AttendanceIssues", null);
            }
        }

        // GET: Schools
        public ActionResult Index()
        {
            int city = Convert.ToInt16(User.Identity.GetCityId());

            var schools = db.Schools.Include(s=>s.People).Where(s=>s.CityId == city);
            return View(schools.ToList());
        }

        // GET: Schools/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SchoolId,Name,DateTimeCreated,CityId")] School school)
        {
            school.DateTimeCreated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                db.Schools.Add(school);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(school);
        }

        // GET: Schools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }

            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SchoolId,Name,DateTimeCreated,CityId")] School school)
        {
            if (ModelState.IsValid)
            {
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(school);
        }

        // GET: Schools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            School school = db.Schools.Find(id);
            db.Schools.Remove(school);
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
