using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _4PsPH.Models
{
    public class FDS
    {
        public int FDSId { get; set; }
        [Display(Name="Family Development Session")]
        public string Name { get; set; }
        public string Body { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public string CreatedBy { get; set; }
        public int CityId { get; set; }
        public virtual City City { get; set; }

        public virtual List<FDSIssue> FDSIssues { get; set; }
    }

    public class FDSIssue
    {
        public int FDSIssueId { get; set; }
        public string Comment { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public int FDSId { get; set; }
        public virtual FDS FDS { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
    }

    //Issue for beneficiaries that are enrolled to a school; preschool to highschool
    public class AttendanceIssue
    {
        public int AttendanceIssueId { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int SchoolId { get; set; }
        public virtual School School { get; set; }

        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Comment { get; set; }
    }

    //Beneficiary annual health checkups and pregnancy tests
    public class HealthCheckupIssue
    {
        public int HealthCheckupIssueId { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }

        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Comment { get; set; }
    }

    // Record all messages received from all mobile numbers; verified and non-verified
    public class Message
    {
        public int MessageId { get; set; }

        public int? MobileNumberId { get; set; }
        public virtual MobileNumber MobileNumber { get; set; }

        public string Body { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        [Display(Name="Category")]
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
    }

    public class Status
    {
        public int StatusId { get; set; }
        [Display(Name = "Status")]
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsEditable { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
    }

    public class Ticket
    {
        public int TicketId { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

        public int? MobileNumberId { get; set; }
        public virtual MobileNumber MobileNumber { get; set; }

        public DateTime DateTimeCreated { get; set; }
        public string IdAttached { get; set; }
        public string Comment { get; set; }
        public bool CreatedAtOffice { get; set; }

        /*for resolution*/
        [Display(Name ="Action Taken / Action Advised")]
        public string ActionAdvised { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string ResolvedBy { get; set; }
        public string ResolvedByUsername { get; set; }

        public virtual List<CaseSummaryReport> CaseSummaryReports { get; set; }
        public virtual List<Endorsement> Endorsements { get; set; }

        public virtual List<TicketComment> TicketComments { get; set; }
    }

    public class TicketComment
    {
        public int TicketCommentId { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public string CreatedByType { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUsername { get; set; }

        [Required]
        public string Body { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }

    public class CaseSummaryReport
    {
        public int CaseSummaryReportId { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        
        public bool? IsApproved { get; set; }
        public string CreatedBy { get; set; }
        
        [AllowHtml]
        public string Body { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime? DateTimeApproved { get; set; }
    }

    public class Endorsement
    {
        public int EndorsementId { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public bool? IsApproved { get; set; }
        public string CreatedBy { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime? DateTimeApproved { get; set; }
    }

    //inquiry categories compliance verification, others
    public class Inquiry
    {
        public int InquiryId { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public DateTime DateTimeCreated { get; set; }
    }
}