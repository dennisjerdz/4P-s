namespace _4PsPH.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<_4PsPH.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(_4PsPH.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            /*seed statuses*/
            context.Statuses.AddOrUpdate(
                s=>s.Name,
                new Status { Name = "Waiting for Verification", Color = "#FFCC80", IsEditable=false },
                new Status { Name = "Verified / Pending Endorsement", Color = "#c7a900", IsEditable = false },
                new Status { Name = "Pending OIC Approval", Color = "#80CBC4", IsEditable = false },
                new Status { Name = "Waiting for Resolution", Color = "#26A69A", IsEditable = false },
                new Status { Name = "Approved", Color = "#388E3C", IsEditable = false },
                new Status { Name = "Declined", Color = "#FF5722", IsEditable = false }
            );

            /*seed roles*/
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            if (!context.Roles.Any(r => r.Name == "Social Worker"))
            {
                var role = new IdentityRole { Name = "Social Worker" };
                roleManager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "OIC"))
            {
                var role = new IdentityRole { Name = "OIC" };
                roleManager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "4P's Officer"))
            {
                var role = new IdentityRole { Name = "4P's Officer" };
                roleManager.Create(role);
            }

            /*seed cities*/
            context.City.AddOrUpdate(
                c => c.Name,
                new City { Name = "Makati", DateTimeCreated = DateTime.UtcNow.AddHours(8) },
                new City { Name = "Pasig", DateTimeCreated = DateTime.UtcNow.AddHours(8) }
            );

            context.SaveChanges();

            int makati_city = context.City.FirstOrDefault(c=>c.Name == "Makati").CityId;
            int pasig_city = context.City.FirstOrDefault(c => c.Name == "Pasig").CityId;

            /*seed accounts*/
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
            };

            if (!context.Users.Any(u => u.UserName == "makati-sw1@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = makati_city,
                    GivenName = "Makati Social Worker 1",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "makati-sw1@gmail.com",
                    Email = "makati-sw1@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "Social Worker");
            }
            if (!context.Users.Any(u => u.UserName == "makati-sw2@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = makati_city,
                    GivenName = "Makati Social Worker 2",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "makati-sw2@gmail.com",
                    Email = "makati-sw2@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "Social Worker");
            }
            if (!context.Users.Any(u => u.UserName == "makati-oic@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = makati_city,
                    GivenName = "Makati OIC",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "makati-oic@gmail.com",
                    Email = "makati-oic@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "OIC");
            }
            if (!context.Users.Any(u => u.UserName == "pasig-sw1@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = pasig_city,
                    GivenName = "Pasig Social Worker 1",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "pasig-sw1@gmail.com",
                    Email = "pasig-sw1@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "Social Worker");
            }
            if (!context.Users.Any(u => u.UserName == "pasig-sw2@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = pasig_city,
                    GivenName = "Pasig Social Worker 2",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "pasig-sw2@gmail.com",
                    Email = "pasig-sw2@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "Social Worker");
            }
            if (!context.Users.Any(u => u.UserName == "pasig-oic@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = pasig_city,
                    GivenName = "Pasig OIC",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "pasig-oic@gmail.com",
                    Email = "pasig-oic@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "OIC");
            }
            if (!context.Users.Any(u => u.UserName == "officer1@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    CityId = makati_city,
                    GivenName = "4P's Officer1",
                    MiddleName = "",
                    LastName = "Test",
                    UserName = "officer1@gmail.com",
                    Email = "officer1@gmail.com",
                    EmailConfirmed = true,
                    IsDisabled = false
                };
                userManager.Create(user, "Testing@123");
                userManager.AddToRole(user.Id, "4P's Officer");
            }

            /*
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            */
        }
    }
}
