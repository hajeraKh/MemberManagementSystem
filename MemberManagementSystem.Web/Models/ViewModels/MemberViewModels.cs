using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class MemberFormViewModel
    {
        public int MemberId { get; set; }

        [Display(Name = "Member ID")]
        public string? MemberCode { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [MaxLength(150)]
        [Display(Name = "Full Name *")]
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
        [Display(Name = "Mobile Number *")]
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

        [Display(Name = "Photo (JPG/PNG)")]
        public IFormFile? PhotoFile { get; set; }

        public string? ExistingPhotoPath { get; set; }

        [Required]
        [Display(Name = "Status *")]
        public string Status { get; set; } = "Active";

        [DataType(DataType.Date)]
        [Display(Name = "Joining Date *")]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    public class MemberDetailsViewModel
    {
        public Member Member { get; set; } = null!;
        public decimal TotalPaidAmount { get; set; }
        public int TotalDocumentsCount { get; set; }
    }
}

