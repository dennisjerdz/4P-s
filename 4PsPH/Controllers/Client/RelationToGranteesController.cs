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
    public class RelationToGranteesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RelationToGrantees
        public ActionResult Index()
        {
            return View(db.RelationToGrantees.ToList());
        }

        // GET: RelationToGrantees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelationToGrantee relationToGrantee = db.RelationToGrantees.Find(id);
            if (relationToGrantee == null)
            {
                return HttpNotFound();
            }
            return View(relationToGrantee);
        }

        // GET: RelationToGrantees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RelationToGrantees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RelationToGranteeId,Name,DateTimeCreated,IsPermanent")] RelationToGrantee relationToGrantee)
        {
            if (ModelState.IsValid)
            {
                db.RelationToGrantees.Add(relationToGrantee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relationToGrantee);
        }

        // GET: RelationToGrantees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelationToGrantee relationToGrantee = db.RelationToGrantees.Find(id);
            if (relationToGrantee == null)
            {
                return HttpNotFound();
            }
            return View(relationToGrantee);
        }

        // POST: RelationToGrantees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RelationToGranteeId,Name,DateTimeCreated,IsPermanent")] RelationToGrantee relationToGrantee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relationToGrantee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relationToGrantee);
        }

        // GET: RelationToGrantees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelationToGrantee relationToGrantee = db.RelationToGrantees.Find(id);
            if (relationToGrantee == null)
            {
                return HttpNotFound();
            }
            return View(relationToGrantee);
        }

        // POST: RelationToGrantees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RelationToGrantee relationToGrantee = db.RelationToGrantees.Find(id);
            db.RelationToGrantees.Remove(relationToGrantee);
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
