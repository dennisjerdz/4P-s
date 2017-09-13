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
    public class HospitalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region 0-5 regular preventive health-checkups and vaccines
        public ActionResult GenerateComplianceForm0to5(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Include(s => s.People).Include(s => s.City).FirstOrDefault(s => s.HospitalId == id);
            if (hospital == null)
            {
                return HttpNotFound();
            }

            return View(hospital);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateComplianceForm0to5([Bind(Include = "HealthCheckupIssues")] Hospital h)
        {
            if (h.HealthCheckupIssues != null)
            {
                foreach (var x in h.HealthCheckupIssues)
                {
                    HealthCheckupIssue hi = new HealthCheckupIssue();
                    hi.ResolvedDate = null;
                    hi.IsResolved = false;
                    string comment = "The beneficiary didn't comply on regular preventive health check-ups and vaccines for month of " + x.Comment + ".";
                    hi.Comment = comment;
                    hi.PersonId = x.PersonId;
                    hi.HospitalId = x.HospitalId;
                    hi.ResolveComment = null;
                    hi.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.HealthCheckupIssues.Add(hi);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
            else
            {
                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
        }
        #endregion

        #region 6-14 receive deworming pills
        public ActionResult GenerateComplianceForm6to14(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Include(s => s.People).Include(s => s.City).FirstOrDefault(s => s.HospitalId == id);
            if (hospital == null)
            {
                return HttpNotFound();
            }

            return View(hospital);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateComplianceForm6to14([Bind(Include = "HealthCheckupIssues")] Hospital h)
        {
            if (h.HealthCheckupIssues != null)
            {
                foreach (var x in h.HealthCheckupIssues)
                {
                    HealthCheckupIssue hi = new HealthCheckupIssue();
                    hi.ResolvedDate = null;
                    hi.IsResolved = false;
                    string comment = "The beneficiary didn't comply on claiming deworming pills for the month of " + x.Comment + ".";
                    hi.Comment = comment;
                    hi.PersonId = x.PersonId;
                    hi.HospitalId = x.HospitalId;
                    hi.ResolveComment = null;
                    hi.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.HealthCheckupIssues.Add(hi);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
            else
            {
                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
        }
        #endregion

        #region PregnantWomenCompliance
        public ActionResult GenerateComplianceFormPregnant(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Include(s => s.People).Include(s => s.City).FirstOrDefault(s => s.HospitalId == id);
            if (hospital == null)
            {
                return HttpNotFound();
            }

            return View(hospital);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateComplianceFormPregnant([Bind(Include = "HealthCheckupIssues")] Hospital h)
        {
            if (h.HealthCheckupIssues != null)
            {
                foreach (var x in h.HealthCheckupIssues)
                {
                    HealthCheckupIssue hi = new HealthCheckupIssue();
                    hi.ResolvedDate = null;
                    hi.IsResolved = false;
                    string comment = "The beneficiary didn't comply on availing pre- and post-natal care for the month of " + x.Comment + ".";
                    hi.Comment = comment;
                    hi.PersonId = x.PersonId;
                    hi.HospitalId = x.HospitalId;
                    hi.ResolveComment = null;
                    hi.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.HealthCheckupIssues.Add(hi);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
            else
            {
                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
        }
        #endregion

        public ActionResult GenerateComplianceForm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Include(s => s.People).Include(s => s.City).FirstOrDefault(s => s.HospitalId == id);
            if (hospital == null)
            {
                return HttpNotFound();
            }

            return View(hospital);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateComplianceForm([Bind(Include = "HealthCheckupIssues")] Hospital h)
        {
            if (h.HealthCheckupIssues != null)
            {
                foreach (var x in h.HealthCheckupIssues)
                {
                    HealthCheckupIssue hi = new HealthCheckupIssue();
                    hi.ResolvedDate = null;
                    hi.IsResolved = false;
                    string comment = "The beneficiary didn't comply for the month of " + x.Comment + ".";
                    hi.Comment = comment;
                    hi.PersonId = x.PersonId;
                    hi.HospitalId = x.HospitalId;
                    hi.ResolveComment = null;
                    hi.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.HealthCheckupIssues.Add(hi);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
            else
            {
                return RedirectToAction("Index", "HealthCheckupIssues", null);
            }
        }

        // GET: Hospitals
        public ActionResult Index()
        {
            if (User.IsInRole("4P's Officer"))
            {
                var hospitals = db.Hospitals.Include(h => h.People);
                return View(hospitals.ToList());
            }
            else
            {
                int city = Convert.ToInt16(User.Identity.GetCityId());

                var hospitals = db.Hospitals.Include(h => h.People).Where(h => h.CityId == city);
                return View(hospitals.ToList());
            }
            
        }

        // GET: Hospitals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // GET: Hospitals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hospitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HospitalId,Name,DateTimeCreated,CityId")] Hospital hospital)
        {
            hospital.DateTimeCreated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                db.Hospitals.Add(hospital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hospital);
        }

        // GET: Hospitals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }

            return View(hospital);
        }

        // POST: Hospitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HospitalId,Name,DateTimeCreated,CityId")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hospital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hospital);
        }

        // GET: Hospitals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // POST: Hospitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hospital hospital = db.Hospitals.Find(id);
            db.Hospitals.Remove(hospital);
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
