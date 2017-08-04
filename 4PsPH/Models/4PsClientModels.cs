﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _4PsPH.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        [Display(Name="Given Name")]
        public string GivenName { get; set; }
        [Display(Name = "Middle Name/Initial")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string getFullName()
        {
            return $"{GivenName} {MiddleName} {LastName}";
        }

        public bool? IsBeneficiary { get; set; }
        public bool? IsGrantee { get; set; }
        public bool? IsParentLeader { get; set; }
        
        public int Gender { get; set; } //0 female, 1 male

        [Display(Name ="Excluded")]
        public bool IsExcluded { get; set; }

        public DateTime BirthDate { get; set; }
        public DateTime DateTimeCreated { get; set; }

        //check 4P's group models
        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        //Refer to PersonInfo
        public int OccupationId { get; set; }
        public virtual Occupation Occupation { get; set; }
        public int EducationalAttainmentId { get; set; }
        public virtual EducationalAttainment EducationalAttainment { get; set; }
        public int RelationToGranteeId { get; set; }
        public virtual RelationToGrantee RelationToGrantee { get; set; }

        public virtual List<MobileNumber> MobileNumbers { get; set; }
    }

    public class MobileNumber
    {
        public int MobileNumberId { get; set; }
        public string MobileNo { get; set; }
        public string Token { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public bool IsDisabled { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        //check 4P's production models
        public virtual List<Message> Messages { get; set; }
    }

    //PersonInfo
    public class Occupation
    {
        public int OccupationId { get; set; }
        [Display(Name ="Occupation")]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [Display(Name ="Ineditable")]
        public bool IsPermanent { get; set; }
    }

    public class EducationalAttainment
    {
        public int EducationalAttainmentId { get; set; }
        [Display(Name = "Educational Attainment")]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [Display(Name = "Ineditable")]
        public bool IsPermanent { get; set; }
    }

    public class RelationToGrantee
    {
        public int RelationToGranteeId { get; set; }
        [Display(Name = "Relation To Grantee")]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [Display(Name = "Ineditable")]
        public bool IsPermanent { get; set; }
    }
    //End of PersonInfo
}