using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Jobsway2goMvc.Models.ViewModel;
using AutoMapper;
using System.Text.RegularExpressions;
using Group = Jobsway2goMvc.Models.Group;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Groups.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            var groupMemberships = await _context.GroupMemberships
                .Include(m => m.User)
                .Where(m => m.GroupId == id && m.IsMember == true)
                .ToListAsync();

            var users = groupMemberships.Select(m => m.User).ToList();
            var membershipList = currentMembership(id);

            var membership = membershipList as ViewResult;
            if (membership == null)
            {
                return Forbid();
            }

            var viewModel = new GroupDetailsViewModel
            {
                GroupId = id,
                Users = users,
                GroupMemberships = groupMemberships,
                CurrentMembershipList = (GroupMembership)membership.Model
            };

            ViewBag.Id = id;

            return View(viewModel);
        }

        public IActionResult currentMembership(int? id)
        {
            if (id == null)
            {
                return Forbid();
            }

            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var membershipList = _context.GroupMemberships
           .FirstOrDefault(m => m.UserId == currentUserId && m.GroupId == id);

            return View(membershipList);
        }

        public async Task<IActionResult> DetailsPostsGroup(int? id)
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
            var membershipList = currentMembership(id);
            var membership = membershipList as ViewResult;

            var viewModel = new GroupDetailsPostsViewModel
            {
                Posts = group.Posts,
                CreatedBy = group.CreatedBy,
                IsPublic = group.IsPublic,
                Name = group.Name,
                Description= group.Description,
                CurrentMembershipList = (GroupMembership)membership.Model
            };

            ViewBag.GroupId = id;
            return View(viewModel);
        }

        public async Task<IActionResult> MemberList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Id = group.Id;
            var users = from u in _userManager.Users
                        join g in _context.GroupMemberships
                        on u.Id equals g.UserId
                        where g.GroupId == id && g.IsMember == true && g.IsBanned == false
                        select u;

            var result = users.ToList();
            return View(users);
        }

        public async Task<IActionResult> AdminList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Id = group.Id;
            var users = from u in _userManager.Users
                        join g in _context.GroupMemberships
                        on u.Id equals g.UserId
                        where g.GroupId == id && g.IsAdmin == true
                        select u;

            var result = users.ToList();
            return View(users);
        }
        public async Task<IActionResult> ModeratorList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Id = group.Id;
            var users = from u in _userManager.Users
                        join g in _context.GroupMemberships
                        on u.Id equals g.UserId
                        where g.GroupId == id && g.IsModerator == true
                        select u;

            var result = users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(string userId, int groupId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            var userExist = _context.GroupMemberships
                .Where(x => x.GroupId == groupId)
                .ToList()
                .Any(x => x.UserId == userId);

            if (userExist)
            {
                ViewBag.MemberExist = "User Already Exists";
                return RedirectToAction("Details", new { id = groupId });
            }

            if (ModelState.IsValid)
            {
                var members = new GroupMembership
                {
                    GroupId = group.Id,
                    UserId = user.Id,
                    IsMember = true,
                    IsAdmin = false,
                    IsModerator = false,
                    IsBanned = false
                };

                _context.GroupMemberships.Add(members);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = groupId });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddModerator(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id && m.IsMember == true);

                if (membership == null)
                {
                    return Forbid();
                }

                var userExist = _context.GroupMemberships
                 .Where(x => x.GroupId == groupId)
                 .ToList()
                 .Any(x => x.UserId == userId && x.IsModerator == true);

                if (userExist)
                {
                    ViewBag.ModeratorExist = "Moderator Already Exists";
                    return RedirectToAction("Details", new { id = groupId });
                }

                if (ModelState.IsValid)
                {
                    membership.IsModerator = true;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveModerator(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id && m.IsMember == true);

                if (membership == null)
                {
                    return Forbid();
                }

                var userExist = _context.GroupMemberships
                 .Where(x => x.GroupId == groupId)
                 .ToList()
                 .Any(x => x.UserId == userId && x.IsModerator == false);

                if (userExist)
                {
                    ViewBag.ModeratorExist = "Moderator does not exist";
                    return RedirectToAction("Details", new { id = groupId });
                }

                if (ModelState.IsValid)
                {
                    membership.IsModerator = false;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id && m.IsMember == true);

                if (membership == null)
                {
                    return Forbid();
                }

                var userExist = _context.GroupMemberships
               .Where(x => x.GroupId == groupId)
               .ToList()
               .Any(x => x.UserId == userId && x.IsAdmin == true);

                if (userExist)
                {
                    ViewBag.AdminExist = "Admin Already Exists";
                    return RedirectToAction("Details", new { id = groupId });
                }

                if (ModelState.IsValid)
                {
                    membership.IsAdmin = true;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id && m.IsMember == true);

                if (membership == null)
                {
                    return Forbid();
                }

                var userExist = _context.GroupMemberships
               .Where(x => x.GroupId == groupId)
               .ToList()
               .Any(x => x.UserId == userId && x.IsAdmin == false);

                if (userExist)
                {
                    ViewBag.AdminExist = "Admin does not Exists";
                    return RedirectToAction("Details", new { id = groupId });
                }

                if (ModelState.IsValid)
                {
                    membership.IsAdmin = false;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult DetailsPost(int id)
        {
            return RedirectToAction("Details", "Post", new { id = id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group @group)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser owner = await _userManager.FindByIdAsync(userid);

            ModelState.Remove("Posts");
            if (ModelState.IsValid)
            {
                var newGroup = new Group
                {
                    Name = @group.Name,
                    CreatedBy = owner.UserName,
                    IsPublic = @group.IsPublic,
                    Description = @group.Description
                };

                _context.Add(newGroup);
                await _context.SaveChangesAsync();

                var membership = new GroupMembership
                {
                    GroupId = newGroup.Id,
                    UserId = owner.Id,
                    Status = Approval.Accepted,
                    IsModerator = true,
                    IsMember = true,
                    IsAdmin = true,
                    IsBanned = false
                };
                _context.Add(membership);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedBy,IsPublic,Description")] Group @group)
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrivacy([FromRoute] int id, [FromBody] bool isPublic)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                try
                {
                    @group.IsPublic = isPublic;
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

            return Ok(@group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Groups == null || _context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Groups' or 'ApplicationDbContext.Posts'  is null.");
            }
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {

                var posts = _context.Posts.Where(p => p.GroupId == id);
                _context.Posts.RemoveRange(posts);

                _context.Groups.Remove(@group);
            }

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

        public async Task<IActionResult> Join(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == user.Id);

            if (membership == null)
            {
                membership = new GroupMembership
                {
                    GroupId = id,
                    UserId = user.Id,
                    Status = Approval.Pending,
                    IsMember = false,
                    IsBanned = false
                };
                _context.Add(membership);
            }
            else
            {
                membership.Status = Approval.Pending;
                _context.Update(membership);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DetailsPostsGroup), new { id });
        }

        public async Task<IActionResult> Requests(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == user.Id && (m.IsAdmin || m.IsModerator));

            if (membership == null)
            {
                return Forbid();
            }

            var requests = _context.GroupMemberships
                .Where(m => m.GroupId == id && m.Status == Approval.Pending)
                .Include(m => m.User)
                .ToList();

            var viewModel = new GroupDetailsViewModel
            {
                GroupMemberships = requests

            };

            ViewBag.GroupId = id;
            return View(viewModel);
        }

        public async Task<IActionResult> Accept(int id, string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == user.Id && (m.IsAdmin || m.IsModerator));

            if (membership == null)
            {
                return Forbid();
            }

            var request = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == userId);

            request.Status = Approval.Accepted;
            request.IsMember = true;
            _context.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Requests), new { id });
        }

        public async Task<IActionResult> Reject(int id, string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == user.Id && (m.IsAdmin || m.IsModerator));

            if (membership == null)
            {
                return Forbid();
            }

            var request = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == userId);

            if (request != null)
            {
                request.Status = Approval.Rejected;
                request.IsMember = false;
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            else
            {
                request = new GroupMembership
                {
                    GroupId = id,
                    UserId = userId,
                    Status = Approval.Pending
                };
                _context.Add(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Requests), new { id });
        }

        public async Task<IActionResult> Leave(int groupId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.UserId == currentUserId && m.GroupId == groupId);
            if (membership == null || (membership.Status != Approval.Accepted && membership.Status != Approval.Pending))
            {
                return NotFound();
            }

            return View(membership);
        }

        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveConfirmed(int groupId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var membership = await _context.GroupMemberships
                .FirstOrDefaultAsync(m => m.UserId == currentUserId && m.GroupId == groupId);
            if (membership == null || (membership.Status != Approval.Accepted && membership.Status != Approval.Pending))
            {
                return NotFound();
            }
            _context.GroupMemberships.Remove(membership);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Ban(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
              .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id);

                if (membership == null)
                {
                    return Forbid();
                }

                var userExist = _context.GroupMemberships
               .Where(x => x.GroupId == groupId)
               .ToList()
               .Any(x => x.UserId == userId && x.IsBanned == true);

                if (userExist)
                {
                    ViewBag.AdminExist = "User Already Banned";
                    return RedirectToAction("Details", new { id = groupId });
                }

                if (ModelState.IsValid)
                {
                    membership.IsBanned = true;
                    membership.IsMember = false;
                    membership.IsModerator = false;
                    membership.IsAdmin = false;
                    membership.Status = Approval.Rejected;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> UnBan(string userId, int groupId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
                if (group == null)
                {
                    return NotFound();
                }

                var membership = await _context.GroupMemberships
              .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == user.Id);

                if (membership == null)
                {
                    return Forbid();
                }

                if (ModelState.IsValid)
                {
                    membership.IsBanned = false;
                    membership.IsMember = true;
                    membership.Status = Approval.Accepted;
                    _context.GroupMemberships.Update(membership);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = groupId });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> BannedMembersList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Id = group.Id;
            var users = from u in _userManager.Users
                        join g in _context.GroupMemberships
                        on u.Id equals g.UserId
                        where g.GroupId == id && g.IsBanned == true
                        select u;

            var result = users.ToList();
            return View(users);
        }

        public async Task<IActionResult> PendingPosts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            ViewBag.Id = group.Id;
            var query = from s in _userManager.Users
                        join p in _context.Posts on s.Id equals p.CreatedById
                        where p.GroupId == id && p.Status == Approval.Pending
                        select new { p.Id, p.Title, p.Description, s.FirstName, s.LastName, p.Status };


            var result = query.ToList();
            var postsList = result.Select(r => new PostListViewModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Status = r.Status
            });

            return View(postsList);
        }

    }
}