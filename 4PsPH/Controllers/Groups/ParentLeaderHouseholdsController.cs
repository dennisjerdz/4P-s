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
    public class ParentLeaderHouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ParentLeaderHouseholds
        public ActionResult Index()
        {
            var parentLeaderHouseholds = db.ParentLeaderHouseholds.Include(p => p.Household).Include(p => p.Person);
            return View(parentLeaderHouseholds.ToList());
        }

        // GET: ParentLeaderHouseholds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentLeaderHousehold parentLeaderHousehold = db.ParentLeaderHouseholds.Find(id);
            if (parentLeaderHousehold == null)
            {
                return HttpNotFound();
            }
            return View(parentLeaderHousehold);
        }

        // GET: ParentLeaderHouseholds/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            return View();
        }

        // POST: ParentLeaderHouseholds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentLeaderHouseholdId,PersonId,HouseholdId,DateTimeCreated")] ParentLeaderHousehold parentLeaderHousehold)
        {
            if (ModelState.IsValid)
            {
                db.ParentLeaderHouseholds.Add(parentLeaderHousehold);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", parentLeaderHousehold.HouseholdId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", parentLeaderHousehold.PersonId);
            return View(parentLeaderHousehold);
        }

        // GET: ParentLeaderHouseholds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentLeaderHousehold parentLeaderHousehold = db.ParentLeaderHouseholds.Find(id);
            if (parentLeaderHousehold == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", parentLeaderHousehold.HouseholdId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", parentLeaderHousehold.PersonId);
            return View(parentLeaderHousehold);
        }

        // POST: ParentLeaderHouseholds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParentLeaderHouseholdId,PersonId,HouseholdId,DateTimeCreated")] ParentLeaderHousehold parentLeaderHousehold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parentLeaderHousehold).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", parentLeaderHousehold.HouseholdId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", parentLeaderHousehold.PersonId);
            return View(parentLeaderHousehold);
        }

        // GET: ParentLeaderHouseholds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentLeaderHousehold parentLeaderHousehold = db.ParentLeaderHouseholds.Find(id);
            if (parentLeaderHousehold == null)
            {
                return HttpNotFound();
            }
            return View(parentLeaderHousehold);
        }

        // POST: ParentLeaderHouseholds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ParentLeaderHousehold parentLeaderHousehold = db.ParentLeaderHouseholds.Find(id);
            db.ParentLeaderHouseholds.Remove(parentLeaderHousehold);
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
