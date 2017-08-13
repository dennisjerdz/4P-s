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

namespace _4PsPH.Controllers
{
    [Authorize(Roles = "Social Worker, 4P's Officer")]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {

            return View(db.Households.Include(h=>h.City).Include(h=>h.People).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Include(h => h.People).Include(h => h.HouseholdHistory).FirstOrDefault(h => h.HouseholdId == id);
            if (household == null)
            {
                return HttpNotFound();
            }

            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            int city = Convert.ToInt16(User.Identity.GetCityId());

            ViewBag.CityId = new SelectList(db.City.Where(c=>c.CityId==city), "CityId", "Name");
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HouseholdId,Name,DateTimeCreated,IsExcluded,CityId")] Household household)
        {
            household.DateTimeCreated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int city = Convert.ToInt16(User.Identity.GetCityId());

            ViewBag.CityId = new SelectList(db.City.Where(c => c.CityId == city), "CityId", "Name");
            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Include(h=>h.HouseholdHistory).FirstOrDefault(h=>h.HouseholdId == id);
            if (household == null)
            {
                return HttpNotFound();
            }
            int city = Convert.ToInt16(User.Identity.GetCityId());

            ViewBag.CityId = new SelectList(db.City.Where(c => c.CityId == city), "CityId", "Name");
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HouseholdId,Name,DateTimeCreated,IsExcluded,CityId")] Household household)
        {
            HouseholdHistory hh = new HouseholdHistory();
            hh.HouseholdId = household.HouseholdId;
            hh.CreatedByUsername = User.Identity.Name;
            hh.CreatedBy = User.Identity.GetFullName();
            hh.Body = "edited Household name to " + household.Name+".";
            hh.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            db.HouseholdHistory.Add(hh);

            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int city = Convert.ToInt16(User.Identity.GetCityId());

            ViewBag.CityId = new SelectList(db.City.Where(c => c.CityId == city), "CityId", "Name");
            return View(household);
        }

        public ActionResult Status(int? id)
        {
            Household household = db.Households.Find(id);

            string stat = null;
            bool new_stat = true;

            if (household.IsExcluded)
            {
                stat = "Active.";
                new_stat = false;
            }
            else
            {
                stat = "Excluded.";
                new_stat = true;
            }

            HouseholdHistory hh = new HouseholdHistory();
            hh.HouseholdId = household.HouseholdId;
            hh.CreatedByUsername = User.Identity.Name;
            hh.CreatedBy = User.Identity.GetFullName();
            hh.Body = "changed the status to "+stat;
            hh.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            db.HouseholdHistory.Add(hh);

            household.IsExcluded = new_stat;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
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
