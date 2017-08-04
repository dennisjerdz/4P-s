﻿using System;
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
    public class PeopleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: People
        public ActionResult Index()
        {
            var persons = db.Persons.Include(p => p.EducationalAttainment).Include(p => p.Household).Include(p => p.Occupation).Include(p => p.RelationToGrantee);
            return View(persons.ToList());
        }

        // GET: People/Details/5
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

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name");
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name");
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name");
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsBeneficiary,IsGrantee,IsParentLeader,Gender,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Persons.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name", person.EducationalAttainmentId);
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", person.HouseholdId);
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name", person.OccupationId);
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name", person.RelationToGranteeId);
            return View(person);
        }

        // GET: People/Edit/5
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
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", person.HouseholdId);
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name", person.OccupationId);
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name", person.RelationToGranteeId);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PersonId,GivenName,MiddleName,LastName,IsBeneficiary,IsGrantee,IsParentLeader,Gender,IsExcluded,BirthDate,DateTimeCreated,HouseholdId,OccupationId,EducationalAttainmentId,RelationToGranteeId")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EducationalAttainmentId = new SelectList(db.EducationalAttainemnts, "EducationalAttainmentId", "Name", person.EducationalAttainmentId);
            ViewBag.HouseholdId = new SelectList(db.Households, "HouseholdId", "Name", person.HouseholdId);
            ViewBag.OccupationId = new SelectList(db.Occupations, "OccupationId", "Name", person.OccupationId);
            ViewBag.RelationToGranteeId = new SelectList(db.RelationToGrantees, "RelationToGranteeId", "Name", person.RelationToGranteeId);
            return View(person);
        }

        // GET: People/Delete/5
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

        // POST: People/Delete/5
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