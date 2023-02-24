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
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace Jobsway2goMvc.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int groupId)
        {
            var posts = await _context.Posts
                .Where(p => p.GroupId == groupId)
                .ToListAsync();

            ViewBag.GroupId = groupId;
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
                .Include(p=> p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.GroupId = id;
            return View(post);
        }

        public IActionResult Create(int groupId)
        {
            var post = new Post { GroupId = groupId };
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,CreatedAtUTC,Type,FirstName,LastName,UserName,ImagePath,CreatedById,GroupId,Status")] Post post)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = await _userManager.FindByIdAsync(userid);
            ModelState.Remove("Group");
            ModelState.Remove("CreatedById");
            ModelState.Remove("Comments");
            ModelState.Remove("Likes");
            if (ModelState.IsValid)
            {
                post.FirstName = user.FirstName;
                post.LastName = user.LastName;
                post.UserName = user.UserName;
                post.ImagePath = user.ImagePath;
                post.CreatedAtUTC = DateTime.Now;
                post.CreatedById = user.Id;
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

            return RedirectToAction("PendingPosts", "Groups", new { id = post.GroupId });
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

            return RedirectToAction("PendingPosts", "Groups", new { id = post.GroupId });
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedAtUTC,Type,FirstName,LastName,UserName,ImagePath,CreatedById,GroupId,Status")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var findPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p=> p.Id == id);
            if(findPost == null)
            {
                return NotFound();
            }
            ModelState.Remove("Group");
            if (ModelState.IsValid)
            {
                    post.Status = findPost.Status;
                    post.ImagePath  = findPost.ImagePath;
                    post.UserName = findPost.UserName;
                    post.LastName = findPost.LastName;
                    post.FirstName = findPost.FirstName;
                    post.CreatedById = findPost.CreatedById;
                    post.CreatedAtUTC = findPost.CreatedAtUTC;
                    post.GroupId = findPost.GroupId;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
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
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(int postId, string text)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(text))
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = await _userManager.FindByIdAsync(userId);

           
                var comment = new Comment
                {
                    Text = text,
                    DateTimeCreated = DateTime.Now,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserImage = user.ImagePath,
                    UserId = userId,
                    PostId = postId
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }
            TempData["Info"] = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                title = "Info",
                message = "Comment succefully created!"
            });
            return RedirectToAction("DetailsPostsGroup", "Groups", new {id = post.GroupId});
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);

            if(comment == null)
            {
                return NotFound();
            }
            var findgroup = await _context.Posts.FindAsync(comment.PostId);

            _context.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["Warning"] = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                title = "Warning",
                message = "Comment succefully deleted!"
            });
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = findgroup.GroupId });
        }

        public async Task<IActionResult> EditComment (int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FindAsync(id);
            if(comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, string text)
        {
           
            var findComment = await _context.Comments.FindAsync(id);
            if (findComment == null)
            {
                return NotFound();
            }
            var findgroup = await _context.Posts.FindAsync(findComment.PostId);
            if (!string.IsNullOrEmpty(text))
            {
                findComment.Text = text;
                await _context.SaveChangesAsync();
                TempData["Info"] = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    title = "Info",
                    message = "Comment succefully updated!"
                });
                return RedirectToAction("DetailsPostsGroup", "Groups", new { id = findgroup.GroupId });
            }
            return View(findComment);
        }

        public async Task<IActionResult> AddLike(int? id)
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
        public async Task<ActionResult> AddLike(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            var like = new Like
            {
                Count = post.Likes.Count + 1,
                PostId = postId,
                UserId = userId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            //TempData["Info"] = Newtonsoft.Json.JsonConvert.SerializeObject(new
            //{
            //    title = "Info",
            //    message = "You !"
            //});
            return RedirectToAction("DetailsPostsGroup", "Groups", new { id = post.GroupId });
        }
    }
}
