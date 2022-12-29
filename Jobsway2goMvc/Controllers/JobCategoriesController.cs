using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Validators.Job_Category;
using FluentValidation.Results;


namespace Jobsway2goMvc.Controllers
{
    public class JobCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.JobCategories.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobCategories == null)
            {
                return NotFound();
            }

            var jobCategory = await _context.JobCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobCategory == null)
            {
                return NotFound();
            }

            return View(jobCategory);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobCategory jobCategory)
        {
            ModelState.Remove("Jobs");

            var validator = new JobCategoryValidator();
            ValidationResult result = validator.Validate(jobCategory);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(jobCategory);
            }

            _context.Add(jobCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobCategories == null)
            {
                return NotFound();
            }

            var jobCategory = await _context.JobCategories.FindAsync(id);
            if (jobCategory == null)
            {
                return NotFound();
            }
            return View(jobCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobCategory jobCategory)
        {
            if (id != jobCategory.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Jobs");

            var validator = new JobCategoryValidator();
            ValidationResult result = validator.Validate(jobCategory);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                return View(jobCategory);
            }

            try
            {
                _context.Update(jobCategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobCategoryExists(jobCategory.Id))
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
            if (id == null || _context.JobCategories == null)
            {
                return NotFound();
            }

            var jobCategory = await _context.JobCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobCategory == null)
            {
                return NotFound();
            }

            return View(jobCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(_context.Jobs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Jobs'  is null.");
            }

            var jobs = _context.Jobs.Where(x => x.CategoryId == id);
            var category = _context.JobCategories.Where(x => x.Id == id);
 
            if (jobs.Count() > 0)
            {
                ViewBag.Delete = "Cannot delete Job Category because it has jobs assigned to it.";
                return View();
            }
            else
            {
                _context.JobCategories.Remove(category.SingleOrDefault());
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        private bool JobCategoryExists(int id)
        {
          return _context.JobCategories.Any(e => e.Id == id);
        }
    }
}
