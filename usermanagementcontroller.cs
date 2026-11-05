using AcxiomCRM.Models;
using AcxiomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                result.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(result);
        }

        public async Task<IActionResult> ManageRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                AvailableRoles = _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToList(),
                SelectedRoles = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(
            ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            model.AvailableRoles = _roleManager.Roles
                .Select(r => r.Name!)
                .ToList();

            if (model.SelectedRoles.Any(
                role => !model.AvailableRoles.Contains(role)))
            {
                ModelState.AddModelError("", "Invalid role selected.");
                return View(model);
            }

            if (user.Id == _userManager.GetUserId(User) &&
                !model.SelectedRoles.Contains("Admin"))
            {
                ModelState.AddModelError("",
                    "You cannot remove your own Admin role.");
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(
                user, currentRoles);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Could not update existing roles.");
                return View(model);
            }

            var addResult = await _userManager.AddToRolesAsync(
                user, model.SelectedRoles.Distinct());

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Could not assign selected roles.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}