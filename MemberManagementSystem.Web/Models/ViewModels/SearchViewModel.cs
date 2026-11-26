using System.Collections.Generic;
using MemberManagementSystem.Web.Models;

namespace MemberManagementSystem.Web.Models.ViewModels
{
    public class SearchViewModel
    {
        public string? Query { get; set; }
        public string? MemberCode { get; set; }
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public string? District { get; set; }
        public string? Division { get; set; }
        public string? Status { get; set; }

        public List<Member> Results { get; set; } = new();
        public int TotalMatches => Results.Count;
    }
}

