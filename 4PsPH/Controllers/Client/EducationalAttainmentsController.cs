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
    public class EducationalAttainmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EducationalAttainments
        public ActionResult Index()
        {
            return View(db.EducationalAttainemnts.ToList());
        }

        // GET: EducationalAttainments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationalAttainment educationalAttainment = db.EducationalAttainemnts.Find(id);
            if (educationalAttainment == null)
            {
                return HttpNotFound();
            }
            return View(educationalAttainment);
        }

        // GET: EducationalAttainments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EducationalAttainments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EducationalAttainmentId,Name,DateTimeCreated,IsPermanent")] EducationalAttainment educationalAttainment)
        {
            if (ModelState.IsValid)
            {
                db.EducationalAttainemnts.Add(educationalAttainment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(educationalAttainment);
        }

        // GET: EducationalAttainments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationalAttainment educationalAttainment = db.EducationalAttainemnts.Find(id);
            if (educationalAttainment == null)
            {
                return HttpNotFound();
            }
            return View(educationalAttainment);
        }

        // POST: EducationalAttainments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EducationalAttainmentId,Name,DateTimeCreated,IsPermanent")] EducationalAttainment educationalAttainment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(educationalAttainment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(educationalAttainment);
        }

        // GET: EducationalAttainments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EducationalAttainment educationalAttainment = db.EducationalAttainemnts.Find(id);
            if (educationalAttainment == null)
            {
                return HttpNotFound();
            }
            return View(educationalAttainment);
        }

        // POST: EducationalAttainments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EducationalAttainment educationalAttainment = db.EducationalAttainemnts.Find(id);
            db.EducationalAttainemnts.Remove(educationalAttainment);
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
