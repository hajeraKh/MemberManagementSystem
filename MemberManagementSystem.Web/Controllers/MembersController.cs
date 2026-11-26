using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemberManagementSystem.Web.Data;
using MemberManagementSystem.Web.Models;
using MemberManagementSystem.Web.Models.ViewModels;
using MemberManagementSystem.Web.Services;

namespace MemberManagementSystem.Web.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public MembersController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: Members
        public async Task<IActionResult> Index(string? status, string? district, string? search)
        {
            var query = _context.Members.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                query = query.Where(m => m.District == district);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(m => m.FullName.Contains(search) || 
                                         m.MobileNumber.Contains(search) || 
                                         m.MemberCode.Contains(search) ||
                                         (m.NationalId != null && m.NationalId.Contains(search)));
            }

            var members = await query
                .OrderByDescending(m => m.MemberId)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentDistrict = district;
            ViewBag.CurrentSearch = search;
            ViewBag.Districts = await _context.Members
                .Where(m => !string.IsNullOrEmpty(m.District))
                .Select(m => m.District!)
                .Distinct()
                .ToListAsync();

            return View(members);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var member = await _context.Members
                .Include(m => m.Documents.OrderByDescending(d => d.UploadDate))
                .Include(m => m.Payments.OrderByDescending(p => p.PaymentDate))
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            var viewModel = new MemberDetailsViewModel
            {
                Member = member,
                TotalPaidAmount = member.Payments.Sum(p => p.Amount),
                TotalDocumentsCount = member.Documents.Count
            };

            return View(viewModel);
        }

        // GET: Members/Create
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Create()
        {
            // Auto generate next member code
            var lastMember = await _context.Members.OrderByDescending(m => m.MemberId).FirstOrDefaultAsync();
            var nextId = (lastMember?.MemberId ?? 1000) + 1;
            var defaultCode = $"MEM-{nextId:D4}";

            var model = new MemberFormViewModel
            {
                MemberCode = defaultCode,
                JoiningDate = DateTime.Today,
                Status = "Active"
            };

            return View(model);
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Create(MemberFormViewModel model)
        {
            if (await _context.Members.AnyAsync(m => m.MemberCode == model.MemberCode))
            {
                ModelState.AddModelError("MemberCode", "Member ID / Code already exists.");
            }

            if (ModelState.IsValid)
            {
                string? photoPath = null;
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var uploadResult = await _fileService.UploadFileAsync(model.PhotoFile, "photos", allowedExts);
                    photoPath = uploadResult.relativePath;
                }

                var member = new Member
                {
                    MemberCode = model.MemberCode ?? $"MEM-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                    FullName = model.FullName.Trim(),
                    FatherName = model.FatherName?.Trim(),
                    MotherName = model.MotherName?.Trim(),
                    DateOfBirth = model.DateOfBirth,
                    NationalId = model.NationalId?.Trim(),
                    MobileNumber = model.MobileNumber.Trim(),
                    Email = model.Email?.Trim(),
                    Address = model.Address?.Trim(),
                    District = model.District?.Trim(),
                    Division = model.Division?.Trim(),
                    PhotoPath = photoPath,
                    Status = model.Status,
                    JoiningDate = model.JoiningDate,
                    Remarks = model.Remarks?.Trim(),
                    CreatedAt = DateTime.Now
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Member '{member.FullName}' ({member.MemberCode}) created successfully!";
                return RedirectToAction(nameof(Details), new { id = member.MemberId });
            }

            return View(model);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            var model = new MemberFormViewModel
            {
                MemberId = member.MemberId,
                MemberCode = member.MemberCode,
                FullName = member.FullName,
                FatherName = member.FatherName,
                MotherName = member.MotherName,
                DateOfBirth = member.DateOfBirth,
                NationalId = member.NationalId,
                MobileNumber = member.MobileNumber,
                Email = member.Email,
                Address = member.Address,
                District = member.District,
                Division = member.Division,
                ExistingPhotoPath = member.PhotoPath,
                Status = member.Status,
                JoiningDate = member.JoiningDate,
                Remarks = member.Remarks
            };

            return View(model);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Edit(int id, MemberFormViewModel model)
        {
            if (id != model.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var member = await _context.Members.FindAsync(id);
                if (member == null)
                {
                    return NotFound();
                }

                // If new photo uploaded
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var uploadResult = await _fileService.UploadFileAsync(model.PhotoFile, "photos", allowedExts);
                    
                    // delete old photo if exists
                    if (!string.IsNullOrEmpty(member.PhotoPath))
                    {
                        _fileService.DeleteFile(member.PhotoPath);
                    }
                    member.PhotoPath = uploadResult.relativePath;
                }

                member.FullName = model.FullName.Trim();
                member.FatherName = model.FatherName?.Trim();
                member.MotherName = model.MotherName?.Trim();
                member.DateOfBirth = model.DateOfBirth;
                member.NationalId = model.NationalId?.Trim();
                member.MobileNumber = model.MobileNumber.Trim();
                member.Email = model.Email?.Trim();
                member.Address = model.Address?.Trim();
                member.District = model.District?.Trim();
                member.Division = model.Division?.Trim();
                member.Status = model.Status;
                member.JoiningDate = model.JoiningDate;
                member.Remarks = model.Remarks?.Trim();
                member.UpdatedAt = DateTime.Now;

                _context.Update(member);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Member '{member.FullName}' updated successfully!";
                return RedirectToAction(nameof(Details), new { id = member.MemberId });
            }

            return View(model);
        }

        // POST: Members/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            member.Status = (member.Status == "Active") ? "Inactive" : "Active";
            member.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Status for {member.FullName} changed to {member.Status}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Members/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members
                .Include(m => m.Documents)
                .Include(m => m.Payments)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // Delete associated files
            if (!string.IsNullOrEmpty(member.PhotoPath))
                _fileService.DeleteFile(member.PhotoPath);

            foreach (var doc in member.Documents)
            {
                _fileService.DeleteFile(doc.FilePath);
            }

            foreach (var pay in member.Payments)
            {
                if (!string.IsNullOrEmpty(pay.ReceiptPath))
                    _fileService.DeleteFile(pay.ReceiptPath);
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Member '{member.FullName}' and all associated records deleted.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Members/Card/5 (Member ID Card)
        public async Task<IActionResult> Card(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            return View(member);
        }
    }
}

