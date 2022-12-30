using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Jobsway2goMvc.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _context.Jobs.Include(j => j.Category).ToListAsync();
            return View(result);
        }
        private ApplicationUser GetApplicationUser(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return user;
        }
        [HttpGet]
        public async Task<IActionResult> ApplyJob(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }
            var job = await _context.Jobs
                 .Include(j => j.Category)
                 .Include(j => j.Applicants)
                 .FirstOrDefaultAsync(m => m.Id == id);                 
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("ApplyJob")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyJobConfirmed(int id)
        {
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Applicants)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (job != null && user != null )
            {
                bool exists = job.Applicants.Any(x => x.Id == user.Id);
                if (exists)
                {
                    ViewBag.JobApplicationExists = "Job Application Exists";
                    return View(job);
                }
                job.Applicants.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }        
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Applicants)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        public IActionResult Create()
        {
            var categories = _context.JobCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,Location,Schedule,Description,OpenSpots,Requirements,DateFrom,DateTo,MinSalary,MaxSalary,CategoryId")] Job job)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Applicants");
           
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = _context.JobCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View(job);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            var categories = _context.JobCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,Location,Schedule,Description,OpenSpots,Requirements,DateFrom,DateTo,MinSalary,MaxSalary,CategoryId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Category");
            ModelState.Remove("Applicants");
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            return View(job);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                 .Include(j => j.Category)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
          return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
