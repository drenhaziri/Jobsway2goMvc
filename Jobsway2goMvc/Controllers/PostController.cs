using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Enums;
using System.Security.Claims;
using System.Timers;

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

        public async Task<IActionResult> Index(int groupId)
        {
            var posts = await _context.Posts
                .Where(p => p.GroupId == groupId)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
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

            return View(post);
        }


        private ApplicationUser GetApplicationUser(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return user;
        }

        public IActionResult Create(int groupId)
        {
            var post = new Post { GroupId = groupId };
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,CreatedAtUTC,Type,GroupId")] Post post)
        {
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Group");
            if (ModelState.IsValid)
            {
                var userAccessor = _httpContextAccessor.HttpContext.User;
                post.CreatedBy = GetApplicationUser(userAccessor);
                post.Status = Approval.Pending;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
            }
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }

        public async Task<IActionResult> Accept(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.Status = Approval.Accepted;
            _context.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }

        public async Task<IActionResult> Reject(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.Status = Approval.Rejected;   
            _context.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
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
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedAtUTC,Type,GroupId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            ModelState.Remove("CreatedBy");
            ModelState.Remove("Group");
            if (ModelState.IsValid)
            {
                try
                {
                    var userAccessor = _httpContextAccessor.HttpContext.User;
                    post.CreatedBy = GetApplicationUser(userAccessor);
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
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
            }
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
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
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
