using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberManagementSystem.Web.Models
{
    [Table("Members")]
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Member ID")]
        public string MemberCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(150)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        [Display(Name = "Father's Name")]
        public string? FatherName { get; set; }

        [MaxLength(150)]
        [Display(Name = "Mother's Name")]
        public string? MotherName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(50)]
        [Display(Name = "National ID (NID)")]
        public string? NationalId { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [MaxLength(30)]
        [Phone]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [MaxLength(300)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [MaxLength(50)]
        [Display(Name = "District")]
        public string? District { get; set; }

        [MaxLength(50)]
        [Display(Name = "Division")]
        public string? Division { get; set; }

        [MaxLength(255)]
        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active, Inactive

        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        // Navigation properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

