using System.Collections.Generic;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int InactiveMembers { get; set; }
        public int NewMembersThisMonth { get; set; }
        public decimal TotalCollections { get; set; }
        public decimal CollectionsThisMonth { get; set; }
        public int TotalDocuments { get; set; }

        public List<Member> RecentMembers { get; set; } = new();
        public List<Payment> RecentPayments { get; set; } = new();
        public List<Document> RecentDocuments { get; set; } = new();

        // Chart Data
        public List<string> MonthlyLabels { get; set; } = new();
        public List<decimal> MonthlyCollections { get; set; } = new();
        public List<string> DistrictLabels { get; set; } = new();
        public List<int> DistrictCounts { get; set; } = new();
    }
}

