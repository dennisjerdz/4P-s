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
    [Authorize]
    public class ClientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult RemoveBeneficiaryStatus(int? id)
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

            person.IsBeneficiary = false;
            db.SaveChanges();

            return RedirectToAction("BeneficiaryAgeCheck");
        }

        public ActionResult BeneficiaryAgeCheck()
        {
            var b = db.Persons.ToList();
            return View(b);
        }

        public ActionResult RemovePregnancy(int? id)
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

            person.IsPregnant = false;
            person.IsBeneficiary = false;

            return RedirectToAction("Details", "Households", new { id = person.HouseholdId });
        }

        // GET: Client
        public ActionResult Index()
        {
            if (User.IsInRole("4P's Officer"))
            {
                var persons = db.Persons.Include(p => p.EducationalAttainment).Include(p => p.Hospital).Include(p => p.Household).Include(p => p.Occupation).Include(p => p.RelationToGrantee).Include(p => p.School);
                return View(persons.ToList());
            }
            else
            {
                int city = Convert.ToInt16(User.Identity.GetCityId());
                var persons = db.Persons.Include(p => p.EducationalAttainment).Include(p => p.Hospital).Include(p => p.Household).Include(p => p.Occupation).Include(p => p.RelationToGrantee).Include(p => p.School).Where(p => p.Household.CityId == city);
                return View(persons.ToList());
            }
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
            ViewBag.O = db.Occupations.ToList();
            ViewBag.EA = db.EducationalAttainemnts.ToList();

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
        public ActionResult Create([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsPregnant,IsBeneficiary,IsGrantee,IsParentLeader,Gender,picture_url,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,SchoolId,HospitalId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            int admin_city = Convert.ToInt16(User.Identity.GetCityId());

            #region occupation and educational attainment
            string getOccupation = Request["O-input"];
            string getEducationalAttainment = Request["EA-input"];

            if (string.IsNullOrWhiteSpace(getOccupation) || string.IsNullOrWhiteSpace(getEducationalAttainment))
            {
                ViewBag.O = db.Occupations.ToList();
                ViewBag.EA = db.EducationalAttainemnts.ToList();

                ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
                ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
                ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
                ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
                ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

                ModelState.AddModelError(string.Empty, "Occupation & Educational Attainment cannot be empty.");
                return View(person);
            }else
            {
                //for occupation create
                if(db.Occupations.Any(o=>o.Name == getOccupation))
                {
                    person.OccupationId = db.Occupations.FirstOrDefault(o => o.Name == getOccupation).OccupationId;
                }else
                {
                    Occupation o = new Occupation();
                    o.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                    o.IsPermanent = false;
                    o.Name = getOccupation;

                    db.Occupations.Add(o);
                    db.SaveChanges();
                    person.OccupationId = o.OccupationId;
                }

                //for educational attainment create
                if (db.EducationalAttainemnts.Any(o => o.Name == getEducationalAttainment))
                {
                    person.EducationalAttainmentId = db.EducationalAttainemnts.FirstOrDefault(o => o.Name == getEducationalAttainment).EducationalAttainmentId;
                }
                else
                {
                    EducationalAttainment e = new EducationalAttainment();
                    e.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                    e.IsPermanent = false;
                    e.Name = getEducationalAttainment;

                    db.EducationalAttainemnts.Add(e);
                    db.SaveChanges();
                    person.EducationalAttainmentId = e.EducationalAttainmentId;
                }

            }
            #endregion

            if (person.IsGrantee == true)
            {
                var check_grantee = db.Persons.FirstOrDefault(p => p.IsGrantee == true && p.HouseholdId == person.HouseholdId);

                if (check_grantee != null)
                {
                    ViewBag.O = db.Occupations.ToList();
                    ViewBag.EA = db.EducationalAttainemnts.ToList();

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

            ViewBag.O = db.Occupations.ToList();
            ViewBag.EA = db.EducationalAttainemnts.ToList();

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
            Person person = db.Persons.FirstOrDefault(p=>p.PersonId == id);
            if (person == null)
            {
                return HttpNotFound();
            }

            int admin_city = Convert.ToInt16(User.Identity.GetCityId());
            ViewBag.O = db.Occupations.ToList();
            ViewBag.EA = db.EducationalAttainemnts.ToList();

            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
            ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
            ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

            return View(person);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsPregnant,IsBeneficiary,IsGrantee,IsParentLeader,Gender,picture_url,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,SchoolId,HospitalId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            int admin_city = Convert.ToInt16(User.Identity.GetCityId());

            #region occupation and educational attainment
            string getOccupation = Request["O-input"];
            string getEducationalAttainment = Request["EA-input"];

            if (string.IsNullOrWhiteSpace(getOccupation) || string.IsNullOrWhiteSpace(getEducationalAttainment))
            {
                ViewBag.O = db.Occupations.ToList();
                ViewBag.EA = db.EducationalAttainemnts.ToList();

                ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
                ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
                ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
                ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
                ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

                ModelState.AddModelError(string.Empty, "Occupation & Educational Attainment cannot be empty.");
                return View(person);
            }
            else
            {
                //for occupation create
                if (db.Occupations.Any(o => o.Name == getOccupation))
                {
                    person.OccupationId = db.Occupations.FirstOrDefault(o => o.Name == getOccupation).OccupationId;
                }
                else
                {
                    Occupation o = new Occupation();
                    o.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                    o.IsPermanent = false;
                    o.Name = getOccupation;

                    db.Occupations.Add(o);
                    db.SaveChanges();
                    person.OccupationId = o.OccupationId;
                }

                //for educational attainment create
                if (db.EducationalAttainemnts.Any(o => o.Name == getEducationalAttainment))
                {
                    person.EducationalAttainmentId = db.EducationalAttainemnts.FirstOrDefault(o => o.Name == getEducationalAttainment).EducationalAttainmentId;
                }
                else
                {
                    EducationalAttainment e = new EducationalAttainment();
                    e.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                    e.IsPermanent = false;
                    e.Name = getEducationalAttainment;

                    db.EducationalAttainemnts.Add(e);
                    db.SaveChanges();
                    person.EducationalAttainmentId = e.EducationalAttainmentId;
                }

            }
            #endregion

            var check_grantee = db.Persons.AsNoTracking().FirstOrDefault(p => p.IsGrantee == true && p.HouseholdId == person.HouseholdId);

            if (person.IsGrantee == true)
            {
                if (check_grantee != null)
                {
                    ViewBag.O = db.Occupations.ToList();
                    ViewBag.EA = db.EducationalAttainemnts.ToList();

                    ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
                    ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
                    ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
                    ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
                    ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

                    ModelState.AddModelError(string.Empty, "A grantee already exists in this household; " + check_grantee.getFullName());
                    return View(person);
                }

            }

            if (ModelState.IsValid)
            {
                HouseholdHistory hh = new HouseholdHistory();
                hh.HouseholdId = person.HouseholdId;
                hh.CreatedByUsername = User.Identity.Name;
                hh.CreatedBy = User.Identity.GetFullName();
                hh.Body = "edited member; " + person.getFullName() + ".";
                hh.DateTimeCreated = DateTime.UtcNow.AddHours(8);
                db.HouseholdHistory.Add(hh);

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Households", new { id = person.HouseholdId });
            }

            ViewBag.O = db.Occupations.ToList();
            ViewBag.EA = db.EducationalAttainemnts.ToList();

            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
            ViewBag.HospitalId = new SelectList(db.Hospitals.Where(s => s.CityId == admin_city), "HospitalId", "Name");
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
            ViewBag.SchoolId = new SelectList(db.Schools.Where(s => s.CityId == admin_city), "SchoolId", "Name");

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
