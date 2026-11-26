using System;
using System.Collections.Generic;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class MemberListReportViewModel
    {
        public string? Status { get; set; }
        public string? District { get; set; }
        public List<Member> Members { get; set; } = new();
    }

    public class NewMembersReportViewModel
    {
        public int Days { get; set; } = 30;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Member> Members { get; set; } = new();
    }

    public class FeeStatusReportItem
    {
        public Member Member { get; set; } = null!;
        public int TotalPaymentsCount { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public string StatusBadge => TotalPaid > 0 ? "Paid" : "Due / Unpaid";
    }

    public class FeeStatusReportViewModel
    {
        public string? Filter { get; set; } // All, Paid, Unpaid
        public List<FeeStatusReportItem> Items { get; set; } = new();
        public decimal GrandTotalCollected { get; set; }
    }

    public class DistrictReportItem
    {
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
    }

    public class DistrictReportViewModel
    {
        public List<DistrictReportItem> Items { get; set; } = new();
        public int TotalMembers { get; set; }
    }

    public class DocumentReportItem
    {
        public Member Member { get; set; } = null!;
        public int TotalDocuments { get; set; }
        public List<Document> Documents { get; set; } = new();
    }

    public class DocumentReportViewModel
    {
        public List<Document> AllDocuments { get; set; } = new();
        public int TotalDocuments { get; set; }
        public Dictionary<string, int> DocumentsByType { get; set; } = new();
    }

    public class MonthlyFinancialItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyFinancialReportViewModel
    {
        public int Year { get; set; }
        public List<MonthlyFinancialItem> MonthlySummary { get; set; } = new();
        public Dictionary<string, decimal> FeeTypeSummary { get; set; } = new();
        public decimal TotalYearCollection { get; set; }
    }
}

