using _4PsPH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _4PsPH.Controllers.Production
{
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Reports
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(c => c.Category).ToList();

            return View(tickets);
        }

        public ActionResult GenerateReportMonth(string form_month_year, string form_month_month)
        {
            //return Content("Month"+form_month_year+"-"+"Year"+form_month_month);

            int month = Convert.ToInt16(form_month_month);
            int year = Convert.ToInt16(form_month_year);

            var tc = db.Tickets
                .Include(t=>t.Category)
                .Include(t=>t.Person)
                .Where(t => t.DateTimeCreated.Month == month && t.DateTimeCreated.Year == year).ToList();
            return PartialView("_reportViewMonth", tc);
        }

        public ActionResult GenerateReportYear(string select_annual)
        {
            //return Content("Month"+form_month_year+"-"+"Year"+form_month_month);

            int year = Convert.ToInt16(select_annual);

            var tc = db.Tickets
                .Include(t => t.Category)
                .Include(t => t.Person)
                .Where(t=>t.DateTimeCreated.Year == year).ToList();
            return PartialView("_reportViewYear", tc);
        }

        public ActionResult Test()
        {
            var tickets = db.Tickets.Include(c=>c.Category).ToList();

            return View(tickets);
        }
    }
}