using _4PsPH.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _4PsPH.Controllers
{
    [Authorize(Roles = "4P's Officer")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Accounts()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users = db.Users.Where(r => r.Email != User.Identity.Name).ToList();
            List<AccountsViewModel> acc = users.Select(u => new AccountsViewModel()
            {
                Email = u.Email,
                City = u.City.Name,
                Name = u.getFullName(),
                IsDisabled = u.IsDisabled,
                Role = String.Join(",", userManager.GetRoles(u.Id))
            }).ToList();

            return View(acc);
        }

        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.City.ToList(), "CityId", "Name");
            ViewBag.RoleList = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        public ActionResult ChangeStatus(string email)
        {
            string c_email = HttpUtility.HtmlDecode(email);
            ApplicationUser user = db.Users.Where(u => u.Email == c_email).FirstOrDefault();

            if(user.IsDisabled){
                user.IsDisabled = false;
            }else
            {
                user.IsDisabled = true;
            }
            
            db.SaveChanges();

            return RedirectToAction("Accounts");
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, CityId = model.CityId, GivenName = model.GivenName, MiddleName = model.MiddleName, LastName = model.LastName, IsDisabled = false };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //login the current logged in user instead of the new one
                    await UserManager.AddToRoleAsync(user.Id, Request.Form["UserRoles"]);
                    return RedirectToAction("Accounts", "Admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            ViewBag.CityId = new SelectList(db.City.ToList(), "CityId", "Name");
            ViewBag.RoleList = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(model);
        }
    }
}