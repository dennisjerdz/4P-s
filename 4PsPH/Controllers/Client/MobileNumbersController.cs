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
    public class MobileNumbersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MobileNumbers
        public ActionResult Index()
        {
            var mobileNumbers = db.MobileNumbers.Include(m => m.Person);
            return View(mobileNumbers.ToList());
        }

        // GET: MobileNumbers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MobileNumber mobileNumber = db.MobileNumbers.Find(id);
            if (mobileNumber == null)
            {
                return HttpNotFound();
            }
            return View(mobileNumber);
        }

        // GET: MobileNumbers/Create
        public ActionResult Create()
        {
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            return View();
        }

        // POST: MobileNumbers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MobileNumberId,MobileNo,Token,DateTimeCreated,IsDisabled,PersonId")] MobileNumber mobileNumber)
        {
            if (ModelState.IsValid)
            {
                db.MobileNumbers.Add(mobileNumber);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", mobileNumber.PersonId);
            return View(mobileNumber);
        }

        // GET: MobileNumbers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MobileNumber mobileNumber = db.MobileNumbers.Find(id);
            if (mobileNumber == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", mobileNumber.PersonId);
            return View(mobileNumber);
        }

        // POST: MobileNumbers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MobileNumberId,MobileNo,Token,DateTimeCreated,IsDisabled,PersonId")] MobileNumber mobileNumber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mobileNumber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", mobileNumber.PersonId);
            return View(mobileNumber);
        }

        // GET: MobileNumbers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MobileNumber mobileNumber = db.MobileNumbers.Find(id);
            if (mobileNumber == null)
            {
                return HttpNotFound();
            }
            return View(mobileNumber);
        }

        // POST: MobileNumbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MobileNumber mobileNumber = db.MobileNumbers.Find(id);
            db.MobileNumbers.Remove(mobileNumber);
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
