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
    public class ResolutionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Resolutions
        public ActionResult Index()
        {
            var resolutions = db.Resolutions.Include(r => r.Ticket);
            return View(resolutions.ToList());
        }

        // GET: Resolutions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resolution resolution = db.Resolutions.Find(id);
            if (resolution == null)
            {
                return HttpNotFound();
            }
            return View(resolution);
        }

        // GET: Resolutions/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached");
            return View();
        }

        // POST: Resolutions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ResolutionId,TicketId,CreatedBy,Body,DateTimeCreated")] Resolution resolution)
        {
            if (ModelState.IsValid)
            {
                db.Resolutions.Add(resolution);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", resolution.TicketId);
            return View(resolution);
        }

        // GET: Resolutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resolution resolution = db.Resolutions.Find(id);
            if (resolution == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", resolution.TicketId);
            return View(resolution);
        }

        // POST: Resolutions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ResolutionId,TicketId,CreatedBy,Body,DateTimeCreated")] Resolution resolution)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resolution).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "TicketId", "IdAttached", resolution.TicketId);
            return View(resolution);
        }

        // GET: Resolutions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resolution resolution = db.Resolutions.Find(id);
            if (resolution == null)
            {
                return HttpNotFound();
            }
            return View(resolution);
        }

        // POST: Resolutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Resolution resolution = db.Resolutions.Find(id);
            db.Resolutions.Remove(resolution);
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
