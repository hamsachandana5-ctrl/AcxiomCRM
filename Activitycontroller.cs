using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var activities = _context.Activities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                activities = activities.Where(a =>
                    a.Subject.Contains(search) ||
                    a.ActivityType.Contains(search) ||
                    a.Status.Contains(search));
            }

            ViewBag.Search = search;

            return View(await activities
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new Activity
            {
                ActivityDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Activity activity)
        {
            if (ModelState.IsValid)
            {
                activity.AssignedTo ??= User.Identity?.Name;

                _context.Activities.Add(activity);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Create",
                    EntityName = "Activity",
                    RecordId = activity.ActivityId.ToString(),
                    NewValue = activity.Subject,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(activity);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Activity activity)
        {
            if (id != activity.ActivityId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Activities.FindAsync(id);

                if (existing == null)
                    return NotFound();

                var oldValue = existing.Subject;

                existing.ActivityType = activity.ActivityType;
                existing.Subject = activity.Subject;
                existing.Description = activity.Description;
                existing.ActivityDate = activity.ActivityDate;
                existing.CustomerId = activity.CustomerId;
                existing.LeadId = activity.LeadId;
                existing.AssignedTo = activity.AssignedTo;
                existing.Status = activity.Status;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Update",
                    EntityName = "Activity",
                    RecordId = id.ToString(),
                    OldValue = oldValue,
                    NewValue = activity.Subject,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(activity);
        }

        public async Task<IActionResult> Details(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Action = "Delete",
                EntityName = "Activity",
                RecordId = id.ToString(),
                OldValue = activity.Subject,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.Activities.Remove(activity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}