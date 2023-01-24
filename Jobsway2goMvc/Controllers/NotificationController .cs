using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController (ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId =  _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            var notifications = _context.Notifications.Where(n=> n.UserName == user.UserName && n.IsRead == false).ToList();
            return Json(notifications);
        }
        [HttpGet]
        public async Task<IActionResult> markedAsRead()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            var notifications = _context.Notifications.Where(n => n.UserName == user.UserName && n.IsRead == true).ToList();
            return Json(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> countNotifications()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);

            var notificationCount = _context.Notifications.Where(n => n.UserName == user.UserName && n.IsRead == false).Count();
            return Json(notificationCount);
        }
        [HttpPost]
        public async Task<IActionResult> markAsRead(int id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if(notification == null)
            {
                return NotFound();
            }
            notification.IsRead =true;
            await _context.SaveChangesAsync();

            return Json(new {success = true});
        }
    }
}
