using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using FluentValidation.Results;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Validators.Sections;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public UserProfileController(UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var user = await _context.Users
                    .Include(g => g.Experiences)
                    .FirstOrDefaultAsync(m => m.Id == userid);
                var profile = _mapper.Map<UserProfileViewModel>(user);
                return View(profile);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm(Name = "file")] IFormFile file)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/images",
                    file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var userid = _userManager.GetUserId(HttpContext.User);
                var user = await _userManager.FindByIdAsync(userid);
                user.ImagePath = "/images/" + file.FileName;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        
        public IActionResult AddExperience()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddExperience(Experience experience)
        {
            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(experience);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(experience);
            }
            
            experience.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Add(experience);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> EditExperience(int? id)
        {
            if (id == null || !_context.Experiences.Any())
            {
                return NotFound();
            }

            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null)
            {
                return NotFound();
            }
            return View(experience);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExperience(int id, Experience experience)
        {
            if (id != experience.Id)
            {
                return NotFound();
            }

            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(experience);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(experience);
            }

            try
            {
                experience.UserId = _userManager.GetUserId(HttpContext.User);
                _context.Update(experience);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceExists(experience.Id))
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
        
        private bool ExperienceExists(int id)
        {
            return _context.Experiences.Any(e => e.Id == id);
        }
        
        public async Task<IActionResult> DeleteExperience(int? id)
        {
            if (id == null || !_context.Experiences.Any())
            {
                return NotFound();
            }

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(m => m.Id == id);
            if (experience == null)
            {
                return NotFound();
            }

            return View(experience);
        }

        [HttpPost, ActionName("DeleteExperience")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Experiences == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Experience'  is null.");
            }
            var experience = await _context.Experiences.FindAsync(id);
            if (experience != null)
            {
                _context.Experiences.Remove(experience);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
