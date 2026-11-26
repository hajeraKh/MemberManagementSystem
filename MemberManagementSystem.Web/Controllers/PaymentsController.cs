using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MemberManagementSystem.Web.Data;
using MemberManagementSystem.Web.Models;
using MemberManagementSystem.Web.Models.ViewModels;
using MemberManagementSystem.Web.Services;

namespace MemberManagementSystem.Web.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public PaymentsController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: Payments
        public async Task<IActionResult> Index(int? memberId, string? feeType, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Payments.Include(p => p.Member).AsQueryable();

            if (memberId.HasValue && memberId > 0)
            {
                query = query.Where(p => p.MemberId == memberId.Value);
            }

            if (!string.IsNullOrWhiteSpace(feeType))
            {
                query = query.Where(p => p.FeeType == feeType);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= toDate.Value);
            }

            var payments = await query.OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.PaymentId).ToListAsync();

            ViewBag.TotalAmount = payments.Sum(p => p.Amount);
            ViewBag.Members = new SelectList(await _context.Members.OrderBy(m => m.FullName).ToListAsync(), "MemberId", "FullName", memberId);
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedFeeType = feeType;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(payments);
        }

        // GET: Payments/Create
        [Authorize(Roles = "Admin,Finance,DataEntry")]
        public async Task<IActionResult> Create(int? memberId, string? returnTo)
        {
            var members = await _context.Members.OrderBy(m => m.FullName).ToListAsync();
            ViewBag.MemberList = new SelectList(members, "MemberId", "FullName", memberId);

            // Generate invoice number e.g. INV-202611-1001
            var today = DateTime.Today;
            var countToday = await _context.Payments.CountAsync(p => p.PaymentDate.Year == today.Year && p.PaymentDate.Month == today.Month);
            var invoiceNo = $"INV-{today:yyyyMM}-{(countToday + 1):D4}";

            var model = new PaymentFormViewModel
            {
                MemberId = memberId ?? 0,
                InvoiceNo = invoiceNo,
                PaymentDate = today,
                FeeType = "Monthly Fee",
                Amount = 1000m,
                PaymentMethod = "Cash",
                ReturnTo = returnTo
            };

            return View(model);
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Finance,DataEntry")]
        public async Task<IActionResult> Create(PaymentFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? receiptPath = null;
                if (model.ReceiptFile != null && model.ReceiptFile.Length > 0)
                {
                    var allowedExts = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    var uploadResult = await _fileService.UploadFileAsync(model.ReceiptFile, "receipts", allowedExts);
                    receiptPath = uploadResult.relativePath;
                }

                var payment = new Payment
                {
                    MemberId = model.MemberId,
                    InvoiceNo = model.InvoiceNo ?? $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                    FeeType = model.FeeType,
                    Amount = model.Amount,
                    PaymentDate = model.PaymentDate,
                    PaymentMethod = model.PaymentMethod,
                    TransactionId = model.TransactionId?.Trim(),
                    ReceiptPath = receiptPath,
                    Remarks = model.Remarks?.Trim(),
                    ReceivedBy = User.Identity?.Name ?? "System",
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Payment of ৳{payment.Amount:N2} recorded successfully! Invoice: {payment.InvoiceNo}";

                if (model.ReturnTo == "member" && model.MemberId > 0)
                {
                    return RedirectToAction("Details", "Members", new { id = model.MemberId });
                }

                return RedirectToAction(nameof(Invoice), new { id = payment.PaymentId });
            }

            var members = await _context.Members.OrderBy(m => m.FullName).ToListAsync();
            ViewBag.MemberList = new SelectList(members, "MemberId", "FullName", model.MemberId);
            return View(model);
        }

        // GET: Payments/Invoice/5 (Money Receipt)
        public async Task<IActionResult> Invoice(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Member)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null || payment.Member == null)
            {
                return NotFound();
            }

            var model = new PaymentInvoiceViewModel
            {
                Payment = payment,
                Member = payment.Member
            };

            return View(model);
        }

        // POST: Payments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, string? returnTo, int? memberId)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            if (!string.IsNullOrEmpty(payment.ReceiptPath))
            {
                _fileService.DeleteFile(payment.ReceiptPath);
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Payment invoice {payment.InvoiceNo} deleted successfully.";

            if (returnTo == "member" && memberId.HasValue)
            {
                return RedirectToAction("Details", "Members", new { id = memberId.Value });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

