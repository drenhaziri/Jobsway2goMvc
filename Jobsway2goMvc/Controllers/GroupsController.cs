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

namespace Jobsway2goMvc.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
              return View(await _context.Groups.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? groupId)
        {
            var user = _userManager.Users.ToList();
            ViewBag.Id = groupId;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(string? userId, int? groupId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if(user == null)
            {
                return NotFound();
            }
            var group = _context.Groups.FirstOrDefault(m => m.Id == groupId);
            if(group == null)
            {
                return NotFound();
            }

            ViewBag.id = groupId; 

            var members = new GroupMembership
            {
                GroupId = group.Id,
                UserId = user.Id
            };

            return View(members);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Group @group)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser owner = _userManager.FindByIdAsync(userid).Result;

            ModelState.Remove("Posts");
            if (ModelState.IsValid)
            {
                _context.Add(new Group
                {
                    Name = @group.Name,
                    CreatedBy = owner.UserName
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

     /*   [HttpGet]
        public JsonResult UserList()
        {
            var user =  _userManager.Users.ToList();
            return new JsonResult(Ok(user));
        }

        */
       
        // GET: Groups/Edit/5
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

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedBy")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

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

        // GET: Groups/Delete/5
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

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Groups == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Groups'  is null.");
            }
            var @group = await _context.Groups.FindAsync(id);
            if (@group != null)
            {
                _context.Groups.Remove(@group);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
          return _context.Groups.Any(e => e.Id == id);
        }
    }
}
