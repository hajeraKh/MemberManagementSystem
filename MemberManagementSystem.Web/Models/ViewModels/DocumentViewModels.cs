using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage = "Please select a Member.")]
        [Display(Name = "Member *")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Document Type is required.")]
        [MaxLength(100)]
        [Display(Name = "Document Type *")]
        public string DocumentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose a file to upload.")]
        [Display(Name = "Upload File (PDF / JPG / PNG) *")]
        public IFormFile? File { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public string? ReturnTo { get; set; }
    }
}

