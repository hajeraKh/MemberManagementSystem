using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(ApplicationDbContext context, IFileService fileService, IWebHostEnvironment env)
        {
            _context = context;
            _fileService = fileService;
            _env = env;
        }

        // GET: Documents
        public async Task<IActionResult> Index(int? memberId, string? docType)
        {
            var query = _context.Documents.Include(d => d.Member).AsQueryable();

            if (memberId.HasValue && memberId > 0)
            {
                query = query.Where(d => d.MemberId == memberId.Value);
            }

            if (!string.IsNullOrWhiteSpace(docType))
            {
                query = query.Where(d => d.DocumentType == docType);
            }

            var docs = await query.OrderByDescending(d => d.UploadDate).ToListAsync();

            ViewBag.Members = new SelectList(await _context.Members.OrderBy(m => m.FullName).ToListAsync(), "MemberId", "FullName", memberId);
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedDocType = docType;

            return View(docs);
        }

        // GET: Documents/Upload
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Upload(int? memberId, string? returnTo)
        {
            var members = await _context.Members.OrderBy(m => m.FullName).ToListAsync();
            ViewBag.MemberList = new SelectList(members, "MemberId", "FullName", memberId);

            var model = new DocumentUploadViewModel
            {
                MemberId = memberId ?? 0,
                ReturnTo = returnTo
            };

            return View(model);
        }

        // POST: Documents/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Upload(DocumentUploadViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a valid file.");
            }

            if (ModelState.IsValid)
            {
                var allowedExts = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc" };
                var uploadResult = await _fileService.UploadFileAsync(model.File!, "documents", allowedExts);

                var doc = new Document
                {
                    MemberId = model.MemberId,
                    DocumentType = model.DocumentType,
                    FileName = uploadResult.originalFileName,
                    FilePath = uploadResult.relativePath,
                    FileSize = uploadResult.fileSize,
                    FileExtension = uploadResult.extension,
                    Remarks = model.Remarks?.Trim(),
                    UploadDate = DateTime.Now
                };

                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Document '{doc.FileName}' uploaded successfully!";

                if (model.ReturnTo == "member" && model.MemberId > 0)
                {
                    return RedirectToAction("Details", "Members", new { id = model.MemberId });
                }

                return RedirectToAction(nameof(Index));
            }

            var members = await _context.Members.OrderBy(m => m.FullName).ToListAsync();
            ViewBag.MemberList = new SelectList(members, "MemberId", "FullName", model.MemberId);
            return View(model);
        }

        // GET: Documents/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativeClean = doc.FilePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRoot, relativeClean);

            if (!System.IO.File.Exists(fullPath))
            {
                TempData["Error"] = "The physical file was not found on server.";
                return RedirectToAction(nameof(Index));
            }

            var contentType = doc.FileExtension?.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, contentType, doc.FileName);
        }

        // POST: Documents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DataEntry")]
        public async Task<IActionResult> Delete(int id, string? returnTo, int? memberId)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return NotFound();

            _fileService.DeleteFile(doc.FilePath);
            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Document deleted successfully.";

            if (returnTo == "member" && memberId.HasValue)
            {
                return RedirectToAction("Details", "Members", new { id = memberId.Value });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

