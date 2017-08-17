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
    public class MobileNumbersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MobileNumbers
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                Household household = db.Households.Include(h => h.People).FirstOrDefault(h => h.HouseholdId == id);
                if (household == null)
                {
                    return HttpNotFound();
                }
                int[] clients = household.People.Select(p => p.PersonId).ToArray();
                var mobileNumbers = db.MobileNumbers
                    .Include(m => m.Messages)
                    .Include(m => m.Person)
                    .Where(m => clients.Contains(m.PersonId)).ToList();

                return View(mobileNumbers);
            }
            else
            {
                int city = Convert.ToInt16(User.Identity.GetCityId());
                var mobileNumbers = db.MobileNumbers
                    .Include(m => m.Messages)
                    .Include(m => m.Person)
                    .Where(m => m.Person.Household.CityId == city).ToList();

                return View(mobileNumbers);
            }
        }

        public ActionResult ClientIndex(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientId = id;
            ViewBag.HouseholdId = person.HouseholdId;
            ViewBag.ClientName = person.getFullName();

            var mobileNumbers = db.MobileNumbers.Include(m => m.Messages).Include(m => m.Person).Where(m => m.PersonId == id).ToList();
            return View(mobileNumbers);
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
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            MobileNumber mb = new MobileNumber();
            mb.PersonId = person.PersonId;

            return View(mb);
        }

        // POST: MobileNumbers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MobileNumberId,MobileNo,Token,DateTimeCreated,IsDisabled,PersonId")] MobileNumber mobileNumber)
        {
            mobileNumber.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            mobileNumber.Token = null;

            if (ModelState.IsValid)
            {
                db.MobileNumbers.Add(mobileNumber);
                db.SaveChanges();
                return RedirectToAction("ClientIndex", new { id = mobileNumber.PersonId });
            }

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
