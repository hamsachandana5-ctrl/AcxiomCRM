
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string status)
        {
            var leads = _context.Leads.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                leads = leads.Where(l =>
                    l.LeadName.Contains(search) ||
                    (l.Email != null && l.Email.Contains(search)) ||
                    (l.CompanyName != null && l.CompanyName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
                leads = leads.Where(l => l.Status == status);

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(await leads.OrderByDescending(l => l.CreatedDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead lead)
        {
            if (lead.ExpectedValue < 0)
                ModelState.AddModelError(nameof(lead.ExpectedValue), "Expected value cannot be negative.");

            if (ModelState.IsValid)
            {
                lead.LeadCode = "LEAD-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
                lead.CreatedDate = DateTime.UtcNow;

                _context.Leads.Add(lead);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Create",
                    EntityName = "Lead",
                    NewValue = lead.LeadName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lead);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lead lead)
        {
            if (id != lead.LeadId) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Leads.FindAsync(id);

                if (existing == null) return NotFound();

                existing.LeadName = lead.LeadName;
                existing.Email = lead.Email;
                existing.Phone = lead.Phone;
                existing.CompanyName = lead.CompanyName;
                existing.Source = lead.Source;
                existing.Status = lead.Status;
                existing.ExpectedValue = lead.ExpectedValue;
                existing.AssignedTo = lead.AssignedTo;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Update",
                    EntityName = "Lead",
                    RecordId = id.ToString(),
                    NewValue = lead.LeadName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lead);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Action = "Delete",
                EntityName = "Lead",
                RecordId = id.ToString(),
                OldValue = lead.LeadName,
                CreatedDate = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.Leads.Remove(lead);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
