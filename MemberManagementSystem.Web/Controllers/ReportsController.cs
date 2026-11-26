using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemberManagementSystem.Web.Data;
using MemberManagementSystem.Web.Models.ViewModels;

namespace MemberManagementSystem.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports Hub
        public IActionResult Index()
        {
            return View();
        }

        // 1. All Members Report (সদস্য তালিকা রিপোর্ট)
        public async Task<IActionResult> MemberList(string? status, string? district, bool export = false)
        {
            var query = _context.Members.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(m => m.Status == status);

            if (!string.IsNullOrWhiteSpace(district))
                query = query.Where(m => m.District == district);

            var members = await query.OrderBy(m => m.MemberCode).ToListAsync();

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("MemberCode,FullName,MobileNumber,NationalId,District,Division,Status,JoiningDate");
                foreach (var m in members)
                {
                    sb.AppendLine($"\"{m.MemberCode}\",\"{m.FullName}\",\"{m.MobileNumber}\",\"{m.NationalId}\",\"{m.District}\",\"{m.Division}\",\"{m.Status}\",\"{m.JoiningDate:yyyy-MM-dd}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"MemberListReport_{DateTime.Now:yyyyMMdd}.csv");
            }

            ViewBag.Status = status;
            ViewBag.District = district;
            ViewBag.Districts = await _context.Members
                .Where(m => !string.IsNullOrEmpty(m.District))
                .Select(m => m.District!)
                .Distinct()
                .ToListAsync();

            var model = new MemberListReportViewModel
            {
                Status = status,
                District = district,
                Members = members
            };

            return View(model);
        }

        // 2. New Members Report (নতুন সদস্য রিপোর্ট)
        public async Task<IActionResult> NewMembers(int days = 30, bool export = false)
        {
            var cutoffDate = DateTime.Today.AddDays(-days);
            var members = await _context.Members
                .Where(m => m.JoiningDate >= cutoffDate)
                .OrderByDescending(m => m.JoiningDate)
                .ToListAsync();

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("MemberCode,FullName,MobileNumber,District,JoiningDate,Status");
                foreach (var m in members)
                {
                    sb.AppendLine($"\"{m.MemberCode}\",\"{m.FullName}\",\"{m.MobileNumber}\",\"{m.District}\",\"{m.JoiningDate:yyyy-MM-dd}\",\"{m.Status}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"NewMembersReport_{DateTime.Now:yyyyMMdd}.csv");
            }

            var model = new NewMembersReportViewModel
            {
                Days = days,
                FromDate = cutoffDate,
                ToDate = DateTime.Today,
                Members = members
            };

            return View(model);
        }

        // 3. Fee Status / Outstanding Report (বকেয়া ফি রিপোর্ট)
        public async Task<IActionResult> FeeStatus(string? filter, bool export = false)
        {
            var members = await _context.Members
                .Include(m => m.Payments)
                .OrderBy(m => m.FullName)
                .ToListAsync();

            var items = members.Select(m => new FeeStatusReportItem
            {
                Member = m,
                TotalPaymentsCount = m.Payments.Count,
                TotalPaid = m.Payments.Sum(p => p.Amount),
                LastPaymentDate = m.Payments.OrderByDescending(p => p.PaymentDate).FirstOrDefault()?.PaymentDate
            }).ToList();

            if (filter == "Paid")
            {
                items = items.Where(i => i.TotalPaid > 0).ToList();
            }
            else if (filter == "Unpaid")
            {
                items = items.Where(i => i.TotalPaid == 0).ToList();
            }

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("MemberCode,FullName,MobileNumber,Status,TotalPaid,PaymentCount,LastPaymentDate");
                foreach (var i in items)
                {
                    sb.AppendLine($"\"{i.Member.MemberCode}\",\"{i.Member.FullName}\",\"{i.Member.MobileNumber}\",\"{i.Member.Status}\",\"{i.TotalPaid:F2}\",\"{i.TotalPaymentsCount}\",\"{i.LastPaymentDate:yyyy-MM-dd}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"FeeStatusReport_{DateTime.Now:yyyyMMdd}.csv");
            }

            var model = new FeeStatusReportViewModel
            {
                Filter = filter,
                Items = items,
                GrandTotalCollected = items.Sum(i => i.TotalPaid)
            };

            return View(model);
        }

        // 4. District-wise Member Report (জেলা ভিত্তিক সদস্য রিপোর্ট)
        public async Task<IActionResult> DistrictWise(bool export = false)
        {
            var items = await _context.Members
                .GroupBy(m => new { District = m.District ?? "Unknown", Division = m.Division ?? "Unknown" })
                .Select(g => new DistrictReportItem
                {
                    District = g.Key.District,
                    Division = g.Key.Division,
                    MemberCount = g.Count(),
                    ActiveCount = g.Count(m => m.Status == "Active"),
                    InactiveCount = g.Count(m => m.Status == "Inactive")
                })
                .OrderByDescending(g => g.MemberCount)
                .ToListAsync();

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Division,District,TotalMembers,ActiveMembers,InactiveMembers");
                foreach (var i in items)
                {
                    sb.AppendLine($"\"{i.Division}\",\"{i.District}\",\"{i.MemberCount}\",\"{i.ActiveCount}\",\"{i.InactiveCount}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"DistrictReport_{DateTime.Now:yyyyMMdd}.csv");
            }

            var model = new DistrictReportViewModel
            {
                Items = items,
                TotalMembers = items.Sum(i => i.MemberCount)
            };

            return View(model);
        }

        // 5. Document Upload Report (ডকুমেন্ট আপলোড রিপোর্ট)
        public async Task<IActionResult> DocumentUploads(bool export = false)
        {
            var docs = await _context.Documents
                .Include(d => d.Member)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();

            var docTypes = docs
                .GroupBy(d => d.DocumentType)
                .ToDictionary(g => g.Key, g => g.Count());

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("MemberCode,MemberName,DocumentType,FileName,UploadDate,FileSize");
                foreach (var d in docs)
                {
                    sb.AppendLine($"\"{d.Member?.MemberCode}\",\"{d.Member?.FullName}\",\"{d.DocumentType}\",\"{d.FileName}\",\"{d.UploadDate:yyyy-MM-dd}\",\"{d.FileSize}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"DocumentUploadReport_{DateTime.Now:yyyyMMdd}.csv");
            }

            var model = new DocumentReportViewModel
            {
                AllDocuments = docs,
                TotalDocuments = docs.Count,
                DocumentsByType = docTypes
            };

            return View(model);
        }

        // 6. Monthly Financial Report (মাসিক আর্থিক রিপোর্ট)
        public async Task<IActionResult> MonthlyFinancial(int? year, bool export = false)
        {
            var targetYear = year ?? DateTime.Today.Year;

            var payments = await _context.Payments
                .Where(p => p.PaymentDate.Year == targetYear)
                .ToListAsync();

            var monthly = new List<MonthlyFinancialItem>();
            for (int m = 1; m <= 12; m++)
            {
                var monthPayments = payments.Where(p => p.PaymentDate.Month == m).ToList();
                monthly.Add(new MonthlyFinancialItem
                {
                    Year = targetYear,
                    Month = m,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    TransactionCount = monthPayments.Count,
                    TotalAmount = monthPayments.Sum(p => p.Amount)
                });
            }

            var feeTypes = payments
                .GroupBy(p => p.FeeType)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            if (export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Month,Year,TransactionsCount,TotalAmount");
                foreach (var m in monthly)
                {
                    sb.AppendLine($"\"{m.MonthName}\",\"{m.Year}\",\"{m.TransactionCount}\",\"{m.TotalAmount:F2}\"");
                }
                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"MonthlyFinancial_{targetYear}_{DateTime.Now:yyyyMMdd}.csv");
            }

            var model = new MonthlyFinancialReportViewModel
            {
                Year = targetYear,
                MonthlySummary = monthly,
                FeeTypeSummary = feeTypes,
                TotalYearCollection = payments.Sum(p => p.Amount)
            };

            return View(model);
        }
    }
}

