using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var logs = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                logs = logs.Where(a =>
                    a.Action.Contains(search) ||
                    a.EntityName.Contains(search) ||
                    (a.UserId != null && a.UserId.Contains(search)));
            }

            ViewBag.Search = search;

            return View(await logs
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var log = await _context.AuditLogs.FindAsync(id);

            if (log == null)
                return NotFound();

            return View(log);
        }
    }
}