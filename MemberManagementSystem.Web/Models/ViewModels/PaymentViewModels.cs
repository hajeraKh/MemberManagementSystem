using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class PaymentFormViewModel
    {
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Please select a Member.")]
        [Display(Name = "Member *")]
        public int MemberId { get; set; }

        [Display(Name = "Invoice No")]
        public string? InvoiceNo { get; set; }

        [Required(ErrorMessage = "Fee Type is required.")]
        [MaxLength(100)]
        [Display(Name = "Fee Type *")]
        public string FeeType { get; set; } = "Monthly Fee";

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, 10000000, ErrorMessage = "Amount must be at least 1.")]
        [Display(Name = "Amount (BDT) *")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date *")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Payment Method is required.")]
        [MaxLength(50)]
        [Display(Name = "Payment Method *")]
        public string PaymentMethod { get; set; } = "Cash";

        [MaxLength(100)]
        [Display(Name = "Transaction ID / Ref")]
        public string? TransactionId { get; set; }

        [Display(Name = "Receipt Upload (PDF / Image)")]
        public IFormFile? ReceiptFile { get; set; }

        public string? ExistingReceiptPath { get; set; }

        [MaxLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public string? ReturnTo { get; set; }
    }

    public class PaymentInvoiceViewModel
    {
        public Payment Payment { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}

