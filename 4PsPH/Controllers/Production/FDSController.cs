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
    public class FDSController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FDS
        public ActionResult Index()
        {
            var fDS = db.FDS.Include(f => f.City);
            return View(fDS.ToList());
        }

        // GET: FDS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDS fDS = db.FDS.Find(id);
            if (fDS == null)
            {
                return HttpNotFound();
            }
            return View(fDS);
        }

        // GET: FDS/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.City, "CityId", "Name");
            return View();
        }

        // POST: FDS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FDSId,Name,Body,EventDate,DateTimeCreated,CreatedBy,CityId")] FDS fDS)
        {
            if (ModelState.IsValid)
            {
                db.FDS.Add(fDS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.City, "CityId", "Name", fDS.CityId);
            return View(fDS);
        }

        // GET: FDS/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDS fDS = db.FDS.Find(id);
            if (fDS == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.City, "CityId", "Name", fDS.CityId);
            return View(fDS);
        }

        // POST: FDS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FDSId,Name,Body,EventDate,DateTimeCreated,CreatedBy,CityId")] FDS fDS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fDS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.City, "CityId", "Name", fDS.CityId);
            return View(fDS);
        }

        // GET: FDS/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDS fDS = db.FDS.Find(id);
            if (fDS == null)
            {
                return HttpNotFound();
            }
            return View(fDS);
        }

        // POST: FDS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FDS fDS = db.FDS.Find(id);
            db.FDS.Remove(fDS);
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
