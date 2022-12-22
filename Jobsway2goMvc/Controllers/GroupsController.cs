using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using System.Security.Claims;

namespace Jobsway2goMvc.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Groups.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .Include(g => g.Posts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            ViewBag.GroupId = id;
            return View(group);
        }

        public IActionResult DetailsPost(int id)
        {
            return RedirectToAction("Details", "Post", new { id = id });
        }

        private ApplicationUser GetApplicationUser(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return user;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Group @group)
        {
            ModelState.Remove("Posts");
            if (ModelState.IsValid)
            {
                var userAccessor = _httpContextAccessor.HttpContext.User;
                var user = GetApplicationUser(userAccessor);

                var groupMembership = new GroupMembership
                {
                    Group = @group,
                    User = user
                };

                _context.Add(@group);
                _context.Add(groupMembership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Posts");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        public IActionResult EditPost(int id)
        {
            return RedirectToAction("Edit", "Post", new { id = id });
        }

        public async Task<IActionResult> Delete([FromQuery] int groupId)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromQuery] int groupId)
        {
            if (_context.Groups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Groups'  is null.");
            }

            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            var groupMemberships = _context.GroupMemberships
                .Where(gm => gm.Group == group)
                .ToList();

            _context.Groups.Remove(group);
            _context.GroupMemberships.RemoveRange(groupMemberships);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult DeletePost(int id)
        {
            return RedirectToAction("Delete", "Post", new { id = id });
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}