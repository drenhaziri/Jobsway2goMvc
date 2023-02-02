using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Data;
using Microsoft.EntityFrameworkCore;
using Jobsway2goMvc.Hubs;
using System.Runtime.Intrinsics.X86;
using System.Linq;

namespace Jobsway2goMvc.Controllers
{
    public class UserConnectionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationHub _notificationHub;
        private readonly IMapper _mapper;
        public UserConnectionController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext ctx, NotificationHub notificationHub)
        {
            _context = ctx;
            _userManager = userManager;
            _mapper = mapper;
            _notificationHub = notificationHub;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        public async Task<IActionResult> Details(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Id = user.Id;

            var membershipList = currentMembershipList(Id);
            var memberShip = membershipList as ViewResult;
            if(memberShip == null)
            {
                return NotFound();
            }

            var profile = _mapper.Map<UserProfileViewModel>(user);
            profile.CurrentConnectionList = (Connection)memberShip.Model;
            return View(profile);

        }

        [HttpPost]
        public async Task<IActionResult> RequestConnection(string Id)
        {

            var connect1 = _userManager.GetUserId(HttpContext.User);
            if (connect1 == null)
            {
                return RedirectToAction("Index", "Privacy");
            }

            var user1 = await _userManager.FindByIdAsync(connect1);

            var user2 = await _userManager.FindByIdAsync(Id);


            if (user1 == null || user2 == null)
            {
                return NotFound();
            }


            if (_context.Connections.Any(c => (c.Connect1 == connect1 && c.Connect2 == Id) ||
                        (c.Connect1 == Id && c.Connect2 == connect1)))
            {
                return BadRequest("A connection already exists between these users.");
            }

            if (ModelState.IsValid)
            {
                var connection = new Connection
                {
                    Connect1 = user1.Id,
                    Connect2 = user2.Id,
                    Status = ConnectionStatus.Pending
                };

                _context.Connections.Add(connection);

                var notification = new Notification
                {
                    UserName = user2.UserName,
                    MessageType = "Personal",
                    IsRead = false,
                    Message = user1.UserName + " sent you a connection request",
                    NotificationDateTime = DateTime.Now,
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                await _notificationHub.SendNotificationToClient(notification.Message, notification.UserName);

                return RedirectToAction(nameof(Details), new { Id });
            }
            return RedirectToAction("Privacy", "Home");
        }

        public IActionResult ConnectionList()
        {
            var connect2 = _userManager.GetUserId(HttpContext.User);
            if (connect2 == null)
            {
                return NotFound(); ;
            }

            var query = from a in _userManager.Users
                        join c in _context.Connections on a.Id equals c.Connect1
                        where c.Connect2 == connect2
                        select new { a.FirstName, a.LastName, UserId =a.Id, c.Status, c.Id };

            var result = query.ToList();
            var connectionList = result.Select(r => new ConnectionListViewModel
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Status = r.Status,
                UserId = r.UserId
            });
            return View(connectionList);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptConnection(int Id)
        {
            var connection = await _context.Connections.FirstOrDefaultAsync(x => x.Id == Id);
            if (connection == null)
            {
                return NotFound();
            }
            var user2 = await _userManager.FindByIdAsync(connection.Connect1);
            var user1 = await _userManager.FindByIdAsync(connection.Connect2);

            connection.Status = ConnectionStatus.Accepted;
           
            var notification = new Notification
            {
                UserName = user2.UserName, 
                MessageType = "Personal",
                IsRead = false,
                Message = user1.UserName + " Just accepted your connection Request!",
                NotificationDateTime = DateTime.Now
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            await _notificationHub.SendNotificationToClient(notification.Message, notification.UserName);

            return RedirectToAction("ConnectionList");
        }

        [HttpPost]
        public async Task<IActionResult> RejectConnection(int Id)
        {

            var connection = await _context.Connections.FirstOrDefaultAsync(x => x.Id == Id);
            if (connection == null)
            {
                return NotFound();
            }
            _context.Remove(connection);
            await _context.SaveChangesAsync();

            return RedirectToAction("ConnectionList");
        }

        public IActionResult currentMembershipList(string? id)
        {
            if (id == null)
            {
                return Forbid();
            }

            var currentUserId = _userManager.GetUserId(HttpContext.User);
            var membershipList = _context.Connections
           .FirstOrDefault(c=> c.Connect1==currentUserId && c.Connect2==id || (c.Connect2==currentUserId && c.Connect1 == id));

            return View(membershipList);
        }
    }
}
