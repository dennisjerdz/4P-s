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
    public class EndorsementCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EndorsementComments
        public ActionResult Index()
        {
            var endorsementComments = db.EndorsementComments.Include(e => e.Endorsement);
            return View(endorsementComments.ToList());
        }

        // GET: EndorsementComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndorsementComment endorsementComment = db.EndorsementComments.Find(id);
            if (endorsementComment == null)
            {
                return HttpNotFound();
            }
            return View(endorsementComment);
        }

        // GET: EndorsementComments/Create
        public ActionResult Create()
        {
            ViewBag.EndorsementId = new SelectList(db.Endorsements, "EndorsementId", "CreatedBy");
            return View();
        }

        // POST: EndorsementComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EndorsementCommentId,EndorsementId,Body,DateTimeCreated")] EndorsementComment endorsementComment)
        {
            if (ModelState.IsValid)
            {
                db.EndorsementComments.Add(endorsementComment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EndorsementId = new SelectList(db.Endorsements, "EndorsementId", "CreatedBy", endorsementComment.EndorsementId);
            return View(endorsementComment);
        }

        // GET: EndorsementComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndorsementComment endorsementComment = db.EndorsementComments.Find(id);
            if (endorsementComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.EndorsementId = new SelectList(db.Endorsements, "EndorsementId", "CreatedBy", endorsementComment.EndorsementId);
            return View(endorsementComment);
        }

        // POST: EndorsementComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EndorsementCommentId,EndorsementId,Body,DateTimeCreated")] EndorsementComment endorsementComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(endorsementComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EndorsementId = new SelectList(db.Endorsements, "EndorsementId", "CreatedBy", endorsementComment.EndorsementId);
            return View(endorsementComment);
        }

        // GET: EndorsementComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EndorsementComment endorsementComment = db.EndorsementComments.Find(id);
            if (endorsementComment == null)
            {
                return HttpNotFound();
            }
            return View(endorsementComment);
        }

        // POST: EndorsementComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EndorsementComment endorsementComment = db.EndorsementComments.Find(id);
            db.EndorsementComments.Remove(endorsementComment);
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
