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
using Microsoft.AspNet.Identity;

namespace _4PsPH.Controllers.Production
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            int city = Convert.ToInt16(User.Identity.GetCityId());

            var tickets = db.Tickets.Include(t => t.Category).Include(t => t.MobileNumber).Include(t => t.Person).Include(t => t.Status).Where(t=>t.Person.Household.CityId == city);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t=>t.TicketComments).FirstOrDefault(t=>t.TicketId == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            TicketComment tc = new TicketComment();
            tc.TicketId = ticket.TicketId;
            tc.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            tc.Ticket = ticket;

            return View(tc);
        }

        // GET: Tickets/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person p = db.Persons.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            Ticket t = new Ticket();
            t.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            t.PersonId = p.PersonId;
            t.Person = p;

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == p.HouseholdId).Select(e=>e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
            return View(t);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,PersonId,CategoryId,MobileNumberId,DateTimeCreated,IdAttached,Comment")] Ticket ticket)
        {
            ticket.CreatedAtOffice = true;
            ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Waiting for Verification").StatusId;

            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
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

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
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

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            int[] member_mobile_no = db.Persons.Where(e => e.HouseholdId == ticket.Person.HouseholdId).Select(e => e.PersonId).ToArray();

            var mb = db.MobileNumbers.Include(m => m.Person).Where(m => member_mobile_no.Contains(m.PersonId) && m.Token != null).Select(s => new SelectListItem
            {
                Value = s.MobileNumberId.ToString(),
                Text = s.MobileNo + " (" + s.Person.GivenName + ")"
            });

            ViewBag.MobileNumberId = new SelectList(mb, "Value", "Text");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "Name");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "TicketCommentId,TicketId,Body,DateTimeCreated")] TicketComment ticketComment)
        {
            ticketComment.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            ticketComment.Ticket = db.Tickets.Include(t => t.TicketComments).FirstOrDefault(t => t.TicketId == ticketComment.TicketId);
            ticketComment.CreatedBy = User.Identity.GetFullName();
            ticketComment.CreatedByUsername = User.Identity.Name;

            string type = "";
            if (User.IsInRole("4P's Officer"))
            {
                type = "4P's Officer";
            }
            if (User.IsInRole("Social Worker"))
            {
                type = "Social Worker";
            }
            if (User.IsInRole("OIC"))
            {
                type = "OIC";
            }

            ticketComment.CreatedByType = type;

            if (ModelState.IsValid)
            {
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            return View(ticketComment);
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
