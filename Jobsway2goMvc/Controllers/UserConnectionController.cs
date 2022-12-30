using AutoMapper;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Data;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Controllers
{
    public class UserConnectionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UserConnectionController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext ctx)
        {
            _context = ctx;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        public async Task<IActionResult> Details(string Id)
        {
            var user = _userManager.FindByIdAsync(Id).Result;
            ViewBag.Id = user.Id;
            var profile = _mapper.Map<UserProfileViewModel>(user);
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

            var user1 = _userManager.FindByIdAsync(connect1);

            var user2 = _userManager.FindByIdAsync(Id);


            if (user1 == null || user2 == null)
            {
                return NotFound();
            }

            var existingConnection = _context.Connections.FirstOrDefault(c => (c.Connect1 == connect1 && c.Connect2 == Id) ||
            (c.Connect1 == Id && c.Connect2 == connect1));

            if (existingConnection != null)
            {
                return BadRequest("A connection already exists between these users.");
            }

            if (ModelState.IsValid)
            {
                var connection = new Connection
                {
                    Connect1 = user1.Result.Id,
                    Connect2 = user2.Result.Id,
                    Status = ConnectionStatus.Pending
                };

                var result = _context.Connections.Add(connection);

                var notification = new Notifications
                {
                    UserName = user2.Result.UserName,
                    MessageType = "Personal",
                    Message = user1.Result.UserName + " sent you a connection request",
                    NotificationDateTime = DateTime.Now,
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Privacy", "Home");
        }

        public async Task<IActionResult> ConnectionList()
        {
            var connect2 = _userManager.GetUserId(HttpContext.User);
            if (connect2 == null)
            {
                return NotFound(); ;
            }

            var query = from a in _userManager.Users
                        join c in _context.Connections on a.Id equals c.Connect1
                        where c.Connect2 == connect2
                        select new { a.FirstName, a.LastName, c.Status, c.Id };

            var result = query.ToList();
            var connectionList = result.Select(r => new ConnectionListViewModel
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Status = r.Status
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
            connection.Status = ConnectionStatus.Accepted;

            await _context.SaveChangesAsync();

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
            connection.Status = ConnectionStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction("ConnectionList");
        }
    }
}
