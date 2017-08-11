using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4PsPH.Models;

namespace _4PsPH.Controllers.Production
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Category).Include(t => t.MobileNumber).Include(t => t.Person).Include(t => t.Status);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            ViewBag.MobileNumberId = new SelectList(db.MobileNumbers, "MobileNumberId", "MobileNo");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,PersonId,CategoryId,StatusId,MobileNumberId,DateTimeCreated,IdAttached,Comment")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", ticket.CategoryId);
            ViewBag.MobileNumberId = new SelectList(db.MobileNumbers, "MobileNumberId", "MobileNo", ticket.MobileNumberId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", ticket.PersonId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", ticket.CategoryId);
            ViewBag.MobileNumberId = new SelectList(db.MobileNumbers, "MobileNumberId", "MobileNo", ticket.MobileNumberId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", ticket.PersonId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name", ticket.StatusId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketId,PersonId,CategoryId,StatusId,MobileNumberId,DateTimeCreated,IdAttached,Comment")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", ticket.CategoryId);
            ViewBag.MobileNumberId = new SelectList(db.MobileNumbers, "MobileNumberId", "MobileNo", ticket.MobileNumberId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "GivenName", ticket.PersonId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name", ticket.StatusId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
