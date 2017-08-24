using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _4PsPH.Models;
using Globe.Connect;
using System.Diagnostics;
using System.Threading.Tasks;

namespace _4PsPH.Controllers
{
    [Authorize]
    public class FDSController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string short_code = "21583313";
        private ActionResult SMS(string mobile_number, string message)
        {
            MobileNumber mb = db.MobileNumbers.FirstOrDefault(m => m.MobileNo == mobile_number);
            string access_token = mb.Token;

            if (access_token != null)
            {
                Sms sms = new Sms(short_code, access_token);

                // mobile number argument is with format 09, convert it to +639
                string globe_format_receiver = "+63" + mobile_number.Substring(1);

                dynamic response = sms.SetReceiverAddress(globe_format_receiver)
                    .SetMessage(message)
                    .SendMessage()
                    .GetDynamicResponse();

                Trace.TraceInformation("Sent message; " + message + " to; " + globe_format_receiver + ".");
            }

            return null;
        }

        public ActionResult GenerateComplianceForm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FDS fds = db.FDS.Include(s => s.City).FirstOrDefault(s => s.FDSId == id);
            if (fds == null)
            {
                return HttpNotFound();
            }

            return View(fds);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GenerateComplianceForm([Bind(Include = "FDSId, FDSIssues")] FDS f)
        {
            if (f.FDSIssues != null)
            {
                foreach (var x in f.FDSIssues)
                {
                    FDSIssue fi = new FDSIssue();
                    fi.ResolvedDate = null;
                    fi.IsResolved = false;
                    string comment = "The grantee didn't attend " + f.Name + ".";
                    fi.Comment = comment;
                    fi.PersonId = x.PersonId;
                    fi.FDSId = f.FDSId;
                    fi.ResolveComment = null;
                    fi.DateTimeCreated = DateTime.UtcNow.AddHours(8);

                    db.FDSIssues.Add(fi);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "FDSIssues", null);
            }
            else
            {
                return RedirectToAction("Index", "FDSIssues", null);
            }
        }

        public ActionResult Broadcast(int? id)
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

            var mbs = db.MobileNumbers.Include(m => m.Person).Where(m => m.Person.Household.CityId == fDS.CityId).ToList();

            foreach(var x in mbs)
            {
                if(x.Token != null)
                {
                    try
                    {
                        string msg = "Hello " + x.Person.GivenName + ", there will be a Family Development Session named "+fDS.Name+" on "+fDS.EventDate+" which would require the Grantee's attendance.";
                        SMS(x.MobileNo, msg);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceInformation("Unable to send message to " + x.MobileNo + " with error; " + e.Message);
                    }
                }
            }

            return RedirectToAction("Index");
        }

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
            return View();
        }

        // POST: FDS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FDSId,Name,Body,EventDate,DateTimeCreated,CreatedBy,CityId")] FDS fDS)
        {
            fDS.DateTimeCreated = DateTime.UtcNow.AddHours(8);
            fDS.CreatedBy = User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.FDS.Add(fDS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
