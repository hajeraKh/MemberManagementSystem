using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberManagementSystem.Web.Models
{
    [Table("Documents")]
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Document Type is required.")]
        [MaxLength(100)]
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; } = string.Empty; // National ID, Passport, Birth Certificate, Educational Certificate, Photo, Other

        [Required]
        [MaxLength(255)]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        [Display(Name = "File Path")]
        public string FilePath { get; set; } = string.Empty;

        [Display(Name = "File Size (Bytes)")]
        public long? FileSize { get; set; }

        [MaxLength(20)]
        [Display(Name = "File Extension")]
        public string? FileExtension { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }
    }
}

