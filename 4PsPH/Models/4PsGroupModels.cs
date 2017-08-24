using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _4PsPH.Models
{
    public class City
    {
        public int CityId { get; set; }
        [Display(Name="City Name")]
        [Required]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public virtual List<Household> Households { get; set; }
        public virtual List<School> Schools { get; set; }

        //check 4P's production models
        public virtual List<FDS> FDS { get; set; }
    }

    public class Household
    {
        public int HouseholdId { get; set; }
        [Required]
        [Display(Name="Household Name")]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [Display(Name = "Excluded")]
        public bool IsExcluded { get; set; }

        public int CityId { get; set; }
        public virtual City City { get; set; }

        public virtual List<Person> People { get; set; }
        public virtual List<HouseholdHistory> HouseholdHistory { get; set; }
    }

    public class HouseholdHistory
    {
        public int HouseholdHistoryId { get; set; }

        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        public string Body { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }

    public class School
    {
        public int SchoolId { get; set; }
        [Display(Name="School Name")]
        [Required]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public int CityId { get; set; }
        public virtual City City { get; set; }

        public virtual List<Person> People { get; set; }
        public virtual List<AttendanceIssue> AttendanceIssues { get; set; }
    }

    public class Hospital
    {
        public int HospitalId { get; set; }
        [Display(Name = "Hospital Name")]
        [Required]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public int CityId { get; set; }
        public virtual City City { get; set; }

        public virtual List<Person> People { get; set; }
        public virtual List<HealthCheckupIssue> HealthCheckupIssues { get; set; }
    }

    public class ParentLeaderHousehold
    {
        public int ParentLeaderHouseholdId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}