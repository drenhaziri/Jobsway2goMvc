using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Jobsway2goMvc.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoleController (RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> UserRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            var userAllRolesList = new List<UserRolesViewModel>();
            foreach (ApplicationUser user in users)
            {
                var userViewModel = new UserRolesViewModel();
                userViewModel.UserId = user.Id;
                userViewModel.Email = user.Email;
                userViewModel.FirstName = user.FirstName;
                userViewModel.LastName = user.LastName;
                userViewModel.Roles = await GetUserRoles(user);
                userAllRolesList.Add(userViewModel);
            }
            return View(userAllRolesList);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new RoleDetailsViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }

            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if(roleName != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(IdentityRole model)
            {

            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.Name;

                // Update the Role using UpdateAsync
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
            }

        }
 }

