
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display and search customers
        public async Task<IActionResult> Index(string search)
        {
            var customers = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)) ||
                    (c.CompanyName != null && c.CompanyName.Contains(search)) ||
                    (c.Phone != null && c.Phone.Contains(search)));
            }

            ViewBag.Search = search;

            return View(await customers
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync());
        }

        // View customer details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // Display create form
        public IActionResult Create()
        {
            return View();
        }

        // Save new customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (await _context.Customers.AnyAsync(c =>
                c.Email == customer.Email || c.Phone == customer.Phone))
            {
                ModelState.AddModelError("", "Email or phone number already exists.");
            }

            if (ModelState.IsValid)
            {
                customer.CustomerCode = "CUS-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
                customer.CreatedDate = DateTime.UtcNow;
                customer.CreatedBy = User.Identity?.Name;

                _context.Customers.Add(customer);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Create",
                    EntityName = "Customer",
                    NewValue = customer.CustomerName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Display edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // Update customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();

            if (await _context.Customers.AnyAsync(c =>
                c.CustomerId != id &&
                (c.Email == customer.Email || c.Phone == customer.Phone)))
            {
                ModelState.AddModelError("", "Email or phone number already exists.");
            }

            if (ModelState.IsValid)
            {
                var existing = await _context.Customers.FindAsync(id);

                if (existing == null) return NotFound();

                existing.CustomerName = customer.CustomerName;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                existing.CompanyName = customer.CompanyName;
                existing.Address = customer.Address;
                existing.City = customer.City;
                existing.State = customer.State;
                existing.Status = customer.Status;

                _context.AuditLogs.Add(new AuditLog
                {
                    UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    Action = "Update",
                    EntityName = "Customer",
                    RecordId = id.ToString(),
                    NewValue = customer.CustomerName,
                    CreatedDate = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // Display delete confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // Delete customer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null) return NotFound();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Action = "Delete",
                EntityName = "Customer",
                RecordId = id.ToString(),
                OldValue = customer.CustomerName,
                CreatedDate = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
