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

namespace _4PsPH.Controllers.Client
{
    [Authorize(Roles = "Social Worker, 4P's Officer")]
    public class ClientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Client
        public ActionResult Index()
        {
            int city = Convert.ToInt16(User.Identity.GetCityId());
            var persons = db.Persons.Include(p => p.EducationalAttainment).Include(p => p.Hospital).Include(p => p.Household).Include(p => p.Occupation).Include(p => p.RelationToGrantee).Include(p => p.School).Where(p=>p.Household.CityId == city);
            return View(persons.ToList());
        }

        // GET: Client/Details/5
        public ActionResult Details(int? id)
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
            return View(person);
        }

        // GET: Client/Create
        public ActionResult Create(int? id)
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

            Person p = new Person();
            p.HouseholdId = household.HouseholdId;
            p.BirthDate = DateTime.UtcNow.AddHours(8);

            int admin_city = Convert.ToInt16(User.Identity.GetCityId());

            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
            ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s=>s.CityId == admin_city), "HospitalId", "Name");
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
            ViewBag.SchoolId = new SelectList(db.Schools.Where(s=>s.CityId == admin_city), "SchoolId", "Name");

            return View(p);
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsBeneficiary,IsGrantee,IsParentLeader,Gender,picture_url,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,SchoolId,HospitalId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            int admin_city = Convert.ToInt16(User.Identity.GetCityId());

            if (person.IsGrantee == true)
            {
                var check_grantee = db.Persons.FirstOrDefault(p => p.IsGrantee == true && p.HouseholdId == person.HouseholdId);

                if (check_grantee != null)
                {
                    ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
                    ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
                    ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
                    ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
                    ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

                    ModelState.AddModelError(string.Empty, "A grantee already exists in this household; " + check_grantee.getFullName());
                    return View(person);
                }
            }
            
            person.DateTimeCreated = DateTime.UtcNow.AddHours(8);

            if (ModelState.IsValid)
            {
                HouseholdHistory hh = new HouseholdHistory();
                hh.HouseholdId = person.HouseholdId;
                hh.CreatedByUsername = User.Identity.Name;
                hh.CreatedBy = User.Identity.GetFullName();
                hh.Body = "added member; "+ person.getFullName() +".";
                hh.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                db.HouseholdHistory.Add(hh);

                db.Persons.Add(person);
                db.SaveChanges();
                return RedirectToAction("Details","Households",new { id = person.HouseholdId });
            }

            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
            ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
            ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

            return View(person);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name", person.EducationalAttainmentId);
            ViewBag.HospitalId = new SelectList(db.Hospitals, "HospitalId", "Name", person.HospitalId);
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", person.HouseholdId);
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name", person.OccupationId);
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name", person.RelationToGranteeId);
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name", person.SchoolId);
            return View(person);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsBeneficiary,IsGrantee,IsParentLeader,Gender,picture_url,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,SchoolId,HospitalId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name", person.EducationalAttainmentId);
            ViewBag.HospitalId = new SelectList(db.Hospitals, "HospitalId", "Name", person.HospitalId);
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", person.HouseholdId);
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name", person.OccupationId);
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name", person.RelationToGranteeId);
            ViewBag.SchoolId = new SelectList(db.Schools, "SchoolId", "Name", person.SchoolId);
            return View(person);
        }

        // GET: Client/Delete/5
        public ActionResult Delete(int? id)
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
            return View(person);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Person person = db.Persons.Find(id);
            db.Persons.Remove(person);
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
