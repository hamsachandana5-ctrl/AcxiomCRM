
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpportunitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string stage)
        {
            var opportunities = _context.Opportunities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                opportunities = opportunities.Where(o =>
                    o.OpportunityName.Contains(search));

            if (!string.IsNullOrWhiteSpace(stage))
                opportunities = opportunities.Where(o => o.Stage == stage);

            ViewBag.Search = search;
            ViewBag.Stage = stage;

            return View(await opportunities
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (opportunity.Status == "Open" &&
                opportunity.ExpectedCloseDate.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError(
                    nameof(opportunity.ExpectedCloseDate),
                    "Expected close date cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                opportunity.CreatedDate = DateTime.UtcNow;

                _context.Opportunities.Add(opportunity);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Create",
                    EntityName = "Opportunity",
                    NewValue = opportunity.OpportunityName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(opportunity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Opportunity opportunity)
        {
            if (id != opportunity.OpportunityId) return NotFound();

            if (opportunity.Status == "Open" &&
                opportunity.ExpectedCloseDate.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError(
                    nameof(opportunity.ExpectedCloseDate),
                    "Expected close date cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _context.Opportunities.FindAsync(id);

                if (existing == null) return NotFound();

                existing.OpportunityName = opportunity.OpportunityName;
                existing.CustomerId = opportunity.CustomerId;
                existing.LeadId = opportunity.LeadId;
                existing.Amount = opportunity.Amount;
                existing.Stage = opportunity.Stage;
                existing.Probability = opportunity.Probability;
                existing.ExpectedCloseDate = opportunity.ExpectedCloseDate;
                existing.Status = opportunity.Status;
                existing.AssignedTo = opportunity.AssignedTo;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Update",
                    EntityName = "Opportunity",
                    RecordId = id.ToString(),
                    NewValue = opportunity.OpportunityName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(opportunity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity == null) return NotFound();

            return View(opportunity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);

            if (opportunity == null) return NotFound();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Action = "Delete",
                EntityName = "Opportunity",
                RecordId = id.ToString(),
                OldValue = opportunity.OpportunityName,
                CreatedDate = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.Opportunities.Remove(opportunity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
