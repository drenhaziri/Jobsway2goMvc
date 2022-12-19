using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jobsway2goMvc.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if(userid == null)
            {
                return RedirectToAction("Index","Home");
            } 
            else
            {
                ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
                UserProfileViewModel userViewModel = new UserProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email =user.Email,
                    CompanyName = user.CompanyName,
                    CompanyArea = user.CompanyArea,
                    ImagePath = user.ImagePath,
                };
                return View(userViewModel);
            }
        }

    }
}
