using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemberManagementSystem.Web.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Invoice / Receipt No")]
        public string InvoiceNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fee Type is required.")]
        [MaxLength(100)]
        [Display(Name = "Fee Type")]
        public string FeeType { get; set; } = string.Empty; // Registration Fee, Monthly Fee, Annual Fee, Renewal Fee, Donation, Other

        [Required(ErrorMessage = "Amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than zero.")]
        [Display(Name = "Amount (BDT)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, bKash, Nagad, Bank Transfer, Cheque

        [MaxLength(100)]
        [Display(Name = "Transaction ID")]
        public string? TransactionId { get; set; }

        [MaxLength(300)]
        [Display(Name = "Receipt / Attachment")]
        public string? ReceiptPath { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [MaxLength(100)]
        [Display(Name = "Received By")]
        public string? ReceivedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }
    }
}

