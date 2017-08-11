using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace _4PsPH.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int CityId { get; set; }
        public virtual City City { get; set; }

        [Required]
        [Display(Name = "Given Name")]
        public string GivenName { get; set; }
        [Display(Name = "Middle Name/Initial")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public bool IsDisabled { get; set; }

        public string getFullName()
        {
            return $"{GivenName} {MiddleName} {LastName}";
        }
    
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //group
        public DbSet<City> City { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<School> Schools { get; set; }

        //client
        public DbSet<Person> Persons  { get; set; }
        public DbSet<MobileNumber> MobileNumbers  { get; set; }
        public DbSet<Occupation> Occupations  { get; set; }
        public DbSet<EducationalAttainment> EducationalAttainemnts  { get; set; }
        public DbSet<RelationToGrantee> RelationToGrantees  { get; set; }

        //production
        public DbSet<FDS> FDS { get; set; }
        public DbSet<FDSIssue> FDSIssues { get; set; }
        public DbSet<AttendanceIssue> AttendanceIssues { get; set; }
        public DbSet<HealthCheckupIssue> HealthCheckupIssues { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<CaseSummaryReport> CaseSummaryReports { get; set; }
        public DbSet<CaseSummaryReportComment> CaseSummaryReportComment { get; set; }
        public DbSet<Endorsement> Endorsements { get; set; }
        public DbSet<EndorsementComment> EndorsementComments { get; set; }
        public DbSet<Resolution> Resolutions { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }

        public System.Data.Entity.DbSet<_4PsPH.Models.ParentLeaderHousehold> ParentLeaderHouseholds { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public System.Data.Entity.DbSet<_4PsPH.Models.Hospital> Hospitals { get; set; }
    }
}