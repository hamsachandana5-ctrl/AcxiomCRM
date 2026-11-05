using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ActivityModel = AcxiomCRM.Models.Activity;

namespace AcxiomCRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalLeads = await _context.Leads.CountAsync();
            ViewBag.OpenLeads = await _context.Leads.CountAsync(l => l.Status != "Converted" && l.Status != "Lost");

            ViewBag.TotalOpportunities = await _context.Opportunities.CountAsync();
            ViewBag.OpenOpportunities = await _context.Opportunities.CountAsync(o => o.Status == "Open");
            ViewBag.WonOpportunities = await _context.Opportunities.CountAsync(o => o.Status == "Won");
            ViewBag.LostOpportunities = await _context.Opportunities.CountAsync(o => o.Status == "Lost");

            ViewBag.TotalPipelineValue = await _context.Opportunities
                .Where(o => o.Status == "Open")
                .SumAsync(o => (decimal?)o.WeightedValue) ?? 0;

            ViewBag.RecentActivities = await _context.Activities
                .OrderByDescending(a => a.ActivityDate)
                .Take(5)
                .ToListAsync();

            ViewBag.LeadStatuses = await _context.Leads
                .GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.OpportunityStages = await _context.Opportunities
                .GroupBy(o => o.Stage)
                .Select(g => new { Stage = g.Key, Count = g.Count() })
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}