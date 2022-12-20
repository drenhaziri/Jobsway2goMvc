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
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(int? groupId)
        {
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);

            var userGroups = _context.GroupMemberships
                .Where(gm => gm.User == user)
                .Select(gm => gm.Group)
                .ToList();

            var posts = _context.Posts
                .Where(p => userGroups.Contains(p.Group));

            if (groupId.HasValue)
            {
                posts = posts.Where(p => p.GroupId == groupId);
            }

            return View(await posts.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Group)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);

            var isMember = _context.GroupMemberships
                .Any(gm => gm.User == user && gm.Group == post.Group);

            if (!isMember)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(post);
        }

        private ApplicationUser GetApplicationUser(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return user;
        }

        public async Task<IActionResult> Create(int? groupId)
        {
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);

            var userGroups = _context.GroupMemberships
                .Where(gm => gm.User == user)
                .Select(gm => gm.Group)
                .ToList();

            if (!userGroups.Any())
            {
                return RedirectToAction("Error", "Home");
            }

            if (groupId.HasValue)
            {
                var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
                if (group == null || !userGroups.Contains(group))
                {
                    return RedirectToAction("Error", "Home");
                }
            }

            ViewBag.GroupId = groupId;
            ViewBag.Groups = new SelectList(userGroups, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int groupId, [Bind("Id,Title,Description,CreatedAtUTC,Type")] Post post)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                var userAccessor = _httpContextAccessor.HttpContext.User;
                post.CreatedBy = GetApplicationUser(userAccessor);
                post.Group = group;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Groups = new SelectList(_context.Groups, "Id", "Name");
            return View(post);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedAtUTC,Type")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                try
                {
                    var userAccessor = _httpContextAccessor.HttpContext.User;
                    post.CreatedBy = GetApplicationUser(userAccessor);
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}