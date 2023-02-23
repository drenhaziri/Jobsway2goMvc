using FluentValidation.Results;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Validators.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Validators.Job_Category;
using Jobsway2goMvc.Validators.Jobs;
using FluentValidation.Results;
using Jobsway2goMvc.Models.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using Jobsway2goMvc.Extensions;
using X.PagedList;
using System.Text.RegularExpressions;
using System.Text;
using Jobsway2goMvc.Enums;

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

        public IActionResult Index(int? page, int itemsPerPage = 6, int pageIndex = 1)
        {
            //Showing 3 jobs per page
            var jobs = _context.Jobs.ToPagedList(page ?? pageIndex, itemsPerPage);

            return View(jobs);
        }

        public IActionResult FilterJobs(JobLocation location, JobPosition position, JobSite site, int minSalary, int maxSalary, int? page, int itemsPerPage = 6, int pageIndex = 1)
        {
            IEnumerable<Job> jobs = _context.Jobs;

            if (location != JobLocation.None)
            {
                jobs = jobs.Where(j => j.Location == location);
            }
            if (position != JobPosition.None)
            {
                jobs = jobs.Where(j => j.Schedule == position);
            }
            if (site != JobSite.None)
            {
                jobs = jobs.Where(j => j.Site == site);
            }
            if (minSalary != 0)
            {
                jobs = jobs.Where(j => j.MinSalary >= minSalary && j.MaxSalary <= maxSalary);
            }

            jobs = jobs.OrderBy(p => p.Id).ToPagedList(page ?? pageIndex, itemsPerPage);


            return View(jobs);
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
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);
            if (User.IsInRole("Business"))
            {
                return NotFound("You can not apply");
            }
            ViewBag.HasAlreadyApplied = job.Applicants.Any(a => a.Id == user.Id);
            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyJob(int id)
        {
            if (User.IsInRole("Business"))
            {
                return NotFound("You can not apply");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);
            job.Applicants.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("ApplyJob", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> UnApplyJob(int id)
        {
            if (User.IsInRole("Business"))
            {
                return NotFound("You can not apply");
            }
            var job = await _context.Jobs
                .Include(j => j.Applicants)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);
            job.Applicants.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("ApplyJob", new { id = id });
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
            ViewBag.Collections = _context.Collections
    .Include(a => a.Jobs)
    .Where(a => a.User.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
    .ToList();

            ViewBag.ShowEditButton = User.IsInRole("Business");
            if (job.Applicants == null || !job.Applicants.Any())
            {
                ViewBag.JobApplication = "There are no applicants for this job";
                return View(job);
            }
            else
            {
                return View(job);
            }

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
            var validator = new JobValidator();
            ValidationResult result = validator.Validate(job);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                var categories = _context.JobCategories.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                return View(job);
            }

            _context.Add(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

            var validator = new JobValidator();
            ValidationResult result = validator.Validate(job);
            ModelState.Remove("Collections");
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                var categories = _context.JobCategories.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(job);
            }

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

        public async Task<IActionResult> SaveToCollection(int? id)
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

            JobCollectionViewModel jobCollection = new JobCollectionViewModel();

            jobCollection.Job = job;

            var collections = _context.Collections
                .Include(a => a.Jobs)
                .Where(a => a.User.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            ViewBag.Collections = new SelectList(collections, "Id", "Name");
            return View(jobCollection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveToCollection(int? id, int? collectionId)
        {
            if (id == null || collectionId == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(c => c.Id == collectionId && c.User.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (collection == null)
            {
                return NotFound();
            }

            if (collection.Jobs.Any(j => j.Id == job.Id))
            {
                TempData["JobExists"] = "Job Exists in Collection";
                return RedirectToAction("Details", new { id = job.Id });
            }

            collection.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}