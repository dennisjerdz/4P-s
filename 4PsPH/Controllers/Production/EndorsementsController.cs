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
    public class EndorsementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Endorsements
        public ActionResult Index()
        {
            var endorsements = db.Endorsements.Include(e => e.Ticket);
            return View(endorsements.ToList());
        }

        // GET: Endorsements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement endorsement = db.Endorsements.Find(id);
            if (endorsement == null)
            {
                return HttpNotFound();
            }
            return View(endorsement);
        }

        // GET: Endorsements/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached");
            return View();
        }

        // POST: Endorsements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EndorsementId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] Endorsement endorsement)
        {
            if (ModelState.IsValid)
            {
                db.Endorsements.Add(endorsement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", endorsement.TicketId);
            return View(endorsement);
        }

        // GET: Endorsements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement endorsement = db.Endorsements.Find(id);
            if (endorsement == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", endorsement.TicketId);
            return View(endorsement);
        }

        // POST: Endorsements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EndorsementId,TicketId,IsApproved,CreatedBy,Body,LastUpdated,DateTimeCreated")] Endorsement endorsement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(endorsement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", endorsement.TicketId);
            return View(endorsement);
        }

        // GET: Endorsements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Endorsement endorsement = db.Endorsements.Find(id);
            if (endorsement == null)
            {
                return HttpNotFound();
            }
            return View(endorsement);
        }

        // POST: Endorsements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Endorsement endorsement = db.Endorsements.Find(id);
            db.Endorsements.Remove(endorsement);
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
