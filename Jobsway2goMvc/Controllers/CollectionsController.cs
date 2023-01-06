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
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using FluentValidation.Results;
using Jobsway2goMvc.Validators.Collections;

namespace Jobsway2goMvc.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signinmanager;

        public CollectionsController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signinmanager)
        {
            _context = context;
            _usermanager = usermanager;
            _signinmanager = signinmanager;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            if (_signinmanager.IsSignedIn(User))
            {
                return View(await _context.Collections
                    .Where(a => a.User.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    .ToListAsync());
            }
            else 
            {
                return NotFound();
            }
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Collections == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .Include(g => g.Jobs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }
            ViewBag.CollectionId = id;
            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        private Task<ApplicationUser> GetCurrentUser() { return _usermanager.GetUserAsync(HttpContext.User); }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Collection collection)
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                ViewBag.NullUser = "User is not logged in";
                return View();
            }
            collection.User = user;

            CollectionValidator validator = new CollectionValidator();
                ValidationResult result = validator.Validate(collection);

                if (!result.IsValid)
                {
                    foreach (ValidationFailure failure in result.Errors)
                    {
                        ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                    }
                }
                else
                {
                    _context.Add(collection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            return View(collection);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Collections == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Collection collection)
        {
            if (id != collection.Id)
            {
                return NotFound();
            }
         
                try
                {
                    var user = await GetCurrentUser();

                    if (user == null)
                    {
                        ViewBag.NullUser = "User is not logged in";
                        return View();  
                    }
                    
                    collection.User = user;

                    CollectionValidator validator = new CollectionValidator();
                    ValidationResult result = validator.Validate(collection);

                    if (!result.IsValid)
                    {
                        foreach (ValidationFailure failure in result.Errors)
                        {
                            ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                        }
                    }
                    else
                    {
                        _context.Update(collection);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            return View(collection);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Collections == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Collections == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Collections'  is null.");
            }
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(int id)
        {
          return _context.Collections.Any(e => e.Id == id);
        }
    }
}
