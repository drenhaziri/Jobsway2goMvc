using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Jobsway2goMvc.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UserProfileController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
                var profile = _mapper.Map<UserProfileViewModel>(user);

                return View(profile);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm(Name = "file")] IFormFile file)
        {           
            if (file.Length > 0)
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);
                using var stream = new FileStream(filepath, FileMode.Create);
                await file.CopyToAsync(stream);
                var userid = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
                user.ImagePath = filepath;
                await _userManager.UpdateAsync(user);
            }            
            return RedirectToAction("Index");
        }

        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
