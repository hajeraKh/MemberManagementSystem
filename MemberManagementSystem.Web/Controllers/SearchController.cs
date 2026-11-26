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
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Search
        public async Task<IActionResult> Index(SearchViewModel model)
        {
            var query = _context.Members.AsQueryable();
            bool hasFilter = false;

            // General keyword search
            if (!string.IsNullOrWhiteSpace(model.Query))
            {
                hasFilter = true;
                var q = model.Query.Trim();
                query = query.Where(m => m.FullName.Contains(q) ||
                                         m.MobileNumber.Contains(q) ||
                                         m.MemberCode.Contains(q) ||
                                         (m.NationalId != null && m.NationalId.Contains(q)) ||
                                         (m.Email != null && m.Email.Contains(q)));
            }

            // Specific Member Code / ID
            if (!string.IsNullOrWhiteSpace(model.MemberCode))
            {
                hasFilter = true;
                query = query.Where(m => m.MemberCode.Contains(model.MemberCode.Trim()));
            }

            // Name
            if (!string.IsNullOrWhiteSpace(model.FullName))
            {
                hasFilter = true;
                query = query.Where(m => m.FullName.Contains(model.FullName.Trim()));
            }

            // Mobile
            if (!string.IsNullOrWhiteSpace(model.MobileNumber))
            {
                hasFilter = true;
                query = query.Where(m => m.MobileNumber.Contains(model.MobileNumber.Trim()));
            }

            // District
            if (!string.IsNullOrWhiteSpace(model.District))
            {
                hasFilter = true;
                query = query.Where(m => m.District == model.District.Trim());
            }

            // Division
            if (!string.IsNullOrWhiteSpace(model.Division))
            {
                hasFilter = true;
                query = query.Where(m => m.Division == model.Division.Trim());
            }

            // Status
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                hasFilter = true;
                query = query.Where(m => m.Status == model.Status.Trim());
            }

            if (hasFilter)
            {
                model.Results = await query
                    .OrderByDescending(m => m.MemberId)
                    .ToListAsync();
            }
            else
            {
                // Default return latest 10 members
                model.Results = await _context.Members
                    .OrderByDescending(m => m.MemberId)
                    .Take(10)
                    .ToListAsync();
            }

            ViewBag.Districts = await _context.Members
                .Where(m => !string.IsNullOrEmpty(m.District))
                .Select(m => m.District!)
                .Distinct()
                .ToListAsync();

            ViewBag.Divisions = await _context.Members
                .Where(m => !string.IsNullOrEmpty(m.Division))
                .Select(m => m.Division!)
                .Distinct()
                .ToListAsync();

            return View(model);
        }
    }
}

