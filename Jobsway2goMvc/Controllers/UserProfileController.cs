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
                    .Include(g => g.Educations)
                    .Include(g => g.Certifications)
                    .Include(g => g.Awards)
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
         public IActionResult AddAward()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult AccountSettings()
        {
            return View();
        }
        public IActionResult EditEmail()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAward(Award award)
        {
            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(award);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(award);
            }
            
            award.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Add(award);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> EditAward(int? id)
        {
            if (id == null || !_context.Awards.Any())
            {
                return NotFound();
            }

            var award = await _context.Awards.FindAsync(id);
            if (award == null)
            {
                return NotFound();
            }
            return View(award);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAward(int id, Award award)
        {
            if (id != award.Id)
            {
                return NotFound();
            }

            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(award);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(award);
            }

            try
            {
                award.UserId = _userManager.GetUserId(HttpContext.User);
                _context.Update(award);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AwardExists(award.Id))
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
        
        /// //
        
        private bool AwardExists(int id)
        {
            return _context.Awards.Any(e => e.Id == id);
        }
        public async Task<IActionResult> DeleteAwards(int? id)
        {
            if (id == null || !_context.Awards.Any())
            {
                return NotFound();
            }

            var award = await _context.Awards 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (award == null)
            {
                return NotFound();
            }

            return View(award);
        }

        [HttpPost, ActionName("DeleteAward")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAwardConfirmed(int id)
        {
            if (_context.Awards== null)
            {
                return Problem("Entity set 'ApplicationDbContext.Award'  is null.");
            }
            var award = await _context.Awards.FindAsync(id);
            if (award!= null)
            {
                _context.Awards.Remove(award);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AddCertification()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddCertification(Certification certification)
        {
            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(certification);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(certification);
            }
            
            certification.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Add(certification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> EditCertification(int? id)
        {
            if (id == null || !_context.Certifications.Any())
            {
                return NotFound();
            }

            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null)
            {
                return NotFound();
            }
            return View(certification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCertification(int id, Certification certification)
        {
            if (id != certification.Id)
            {
                return NotFound();
            }

            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(certification);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(certification);
            }

            try
            {
                certification.UserId = _userManager.GetUserId(HttpContext.User);
                _context.Update(certification);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificationExists(certification.Id))
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
        
        private bool CertificationExists(int id)
        {
            return _context.Certifications.Any(e => e.Id == id);
        }
        public async Task<IActionResult> DeleteCertifications(int? id)
        {
            if (id == null || !_context.Certifications.Any())
            {
                return NotFound();
            }

            var certification = await _context.Certifications 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certification == null)
            {
                return NotFound();
            }

            return View(certification);
        }

        [HttpPost, ActionName("DeleteCertification")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCertificationConfirmed(int id)
        {
            if (_context.Certifications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Certification'  is null.");
            }
            var certification = await _context.Certifications.FindAsync(id);
            if (certification!= null)
            {
                _context.Certifications.Remove(certification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AddEducation()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddEducation(Education education)
        {
            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(education);
            
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(education);
            }
            
            education.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Add(education);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> EditEducation(int? id)
        {
            if (id == null || !_context.Educations.Any())
            {
                return NotFound();
            }

            var education = await _context.Educations.FindAsync(id);
            if (education == null)
            {
                return NotFound();
            }
            return View(education);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEducation(int id, Education education)
        {
            if (id != education.Id)
            {
                return NotFound();
            }

            var validator = new SectionValidator();
            ValidationResult result = validator.Validate(education);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(education);
            }

            try
            {
                education.UserId = _userManager.GetUserId(HttpContext.User);
                _context.Update(education);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationExists(education.Id))
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
        
        private bool EducationExists(int id)
        {
            return _context.Educations.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteEducation(int? id)
        {
            if (id == null || !_context.Educations.Any())
            {
                return NotFound();
            }

            var education = await _context.Educations 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (education == null)
            {
                return NotFound();
            }

            return View(education);
        }

        [HttpPost, ActionName("DeleteEducation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEducationConfirmed(int id)
        {
            if (_context.Educations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Education'  is null.");
            }
            var education = await _context.Educations.FindAsync(id);
            if (education != null)
            {
                _context.Educations.Remove(education);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
