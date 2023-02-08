﻿using FluentValidation.Results;
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
            var userAccessor = _httpContextAccessor.HttpContext.User;
            var user = GetApplicationUser(userAccessor);
            ViewBag.HasAlreadyApplied = job.Applicants.Any(a => a.Id == user.Id);
            return View(job);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> ApplyJob(int id)
        {
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
        public async Task<IActionResult> SaveToCollection(Job job, Collection collection)
        {
            var collectionRef = await _context.Collections
                .Include(a => a.Jobs)
                .FirstOrDefaultAsync(a => a.Id == collection.Id);
            var jobRef = await _context.Jobs.FirstOrDefaultAsync(a => a.Id == job.Id);

            if (collectionRef != null && jobRef != null)
            {
                bool exists = collectionRef.Jobs.Any(x => x.Id == job.Id);
                if (exists)
                {
                    ViewBag.JobExists = "Job Exists in Collection";
                    return View(job);
                }
                collectionRef.Jobs.Add(jobRef);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Jobs");
        }

        private bool JobExists(int id)
        {
          return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
