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

        // GET: JobCategories
        public async Task<IActionResult> Index()
        {
              return View(await _context.JobCategories.ToListAsync());
        }

        // GET: JobCategories/Details/5
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

        // GET: JobCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: JobCategories/Edit/5
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

        // POST: JobCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    
        // GET: JobCategories/Delete/5
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

        // POST: JobCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobCategories'  is null.");
            }
            var jobCategory = await _context.JobCategories.FindAsync(id);
            if (jobCategory != null)
            {
                _context.JobCategories.Remove(jobCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobCategoryExists(int id)
        {
          return _context.JobCategories.Any(e => e.Id == id);
        }
    }
}
