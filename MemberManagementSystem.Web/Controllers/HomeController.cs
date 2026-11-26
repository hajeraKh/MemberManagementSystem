using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemberManagementSystem.Web.Data;
using MemberManagementSystem.Web.Models.ViewModels;

namespace MemberManagementSystem.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Today;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var totalMembers = await _context.Members.CountAsync();
            var activeMembers = await _context.Members.CountAsync(m => m.Status == "Active");
            var inactiveMembers = totalMembers - activeMembers;
            var newMembersThisMonth = await _context.Members.CountAsync(m => m.JoiningDate >= startOfMonth);

            var totalCollections = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0m;
            var collectionsThisMonth = await _context.Payments
                .Where(p => p.PaymentDate >= startOfMonth)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            var totalDocuments = await _context.Documents.CountAsync();

            var recentMembers = await _context.Members
                .OrderByDescending(m => m.MemberId)
                .Take(5)
                .ToListAsync();

            var recentPayments = await _context.Payments
                .Include(p => p.Member)
                .OrderByDescending(p => p.PaymentId)
                .Take(5)
                .ToListAsync();

            var recentDocuments = await _context.Documents
                .Include(d => d.Member)
                .OrderByDescending(d => d.DocumentId)
                .Take(5)
                .ToListAsync();

            // Monthly collections chart data (past 6 months)
            var monthlyLabels = new System.Collections.Generic.List<string>();
            var monthlyCollections = new System.Collections.Generic.List<decimal>();

            for (int i = 5; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthTotal = await _context.Payments
                    .Where(p => p.PaymentDate >= monthStart && p.PaymentDate <= monthEnd)
                    .SumAsync(p => (decimal?)p.Amount) ?? 0m;

                monthlyLabels.Add(targetMonth.ToString("MMM yyyy", CultureInfo.InvariantCulture));
                monthlyCollections.Add(monthTotal);
            }

            // District breakdown chart data
            var districtGroup = await _context.Members
                .Where(m => !string.IsNullOrEmpty(m.District))
                .GroupBy(m => m.District)
                .Select(g => new { District = g.Key!, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(6)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                InactiveMembers = inactiveMembers,
                NewMembersThisMonth = newMembersThisMonth,
                TotalCollections = totalCollections,
                CollectionsThisMonth = collectionsThisMonth,
                TotalDocuments = totalDocuments,
                RecentMembers = recentMembers,
                RecentPayments = recentPayments,
                RecentDocuments = recentDocuments,
                MonthlyLabels = monthlyLabels,
                MonthlyCollections = monthlyCollections,
                DistrictLabels = districtGroup.Select(d => d.District).ToList(),
                DistrictCounts = districtGroup.Select(d => d.Count).ToList()
            };

            return View(viewModel);
        }
    }
}
