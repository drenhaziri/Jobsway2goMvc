using FluentValidation.Results;
using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Hubs;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Jobsway2goMvc.Validators.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Jobsway2goMvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationHub _notificationHub;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, NotificationHub notificationHub)
        {
            _context = context;
            _userManager = userManager;
            _notificationHub = notificationHub;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        public IActionResult Create()
        {
            var eventModel = new Event();
            var selectList = new List<SelectListItem>();
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            foreach (var user in _userManager.Users.Where(u => u.Id != currentUserId))
            {
                selectList.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.UserName
                });
            }
            ViewBag.Users = selectList;
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event, List<string> guestsIds)
        {
            var validator = new EventValidator();
            ValidationResult result = validator.Validate(@event);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                var selectList = new List<SelectListItem>();
                var currentUserId = _userManager.GetUserId(HttpContext.User);
                foreach (var user in _userManager.Users.Where(u => u.Id != currentUserId))
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = user.Id,
                        Text = user.UserName
                    });
                }
                ViewBag.Users = selectList;
                return View(@event);
            }
            var guests = _userManager.Users.Where(u => guestsIds.Contains(u.Id)).ToList();
            foreach (var guest in guests)
            {
                var guestEvent = new EventGuest();
                guestEvent.Event = @event;
                guestEvent.ApplicationUser = guest;
                guestEvent.Status = EApproval.Pending;
                _context.EventGuests.Add(guestEvent);

                string message = $"You have been invited to the '{@event.Title}' event.";
                await _notificationHub.SendNotificationToClient(message, guest.UserName);
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser creator = _userManager.FindByIdAsync(userid).Result;

            @event.CreatedBy = creator.Id;

            _context.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            var viewModel = new EventViewModel
            {
                Event = @event,
                UserManager = _userManager,
                EventGuests = await _context.EventGuests.Where(x => x.EventId == id).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            var validator = new EventValidator();
            ValidationResult result = validator.Validate(@event);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                var viewModel = new EventViewModel
                {
                    Event = @event,
                    UserManager = _userManager,
                    EventGuests = await _context.EventGuests.Where(x => x.EventId == id).ToListAsync()
                };
                return View(viewModel);
            }

            List<string> guestsIds = Request.Form["guestsIds"].ToList();
            var currentGuests = _context.EventGuests.Where(eg => eg.EventId == id).ToList();

            List<EventGuest> guestsRemoved = currentGuests.Where(guest => !guestsIds.Contains(guest.GuestId)).ToList();
            foreach (var guest in guestsRemoved)
            {
                _context.EventGuests.Remove(guest);
                currentGuests.Remove(guest);
            }

            var newGuests = _userManager.Users.AsEnumerable().Where(user => guestsIds.Contains(user.Id) && !currentGuests.Any(guest => guest.GuestId == user.Id)).ToList();
            foreach (var guest in newGuests)
            {
                var guestEvent = new EventGuest();
                guestEvent.Event = @event;
                guestEvent.ApplicationUser = guest;
                guestEvent.Status = EApproval.Pending;
                _context.EventGuests.Add(guestEvent);
                currentGuests.Add(guestEvent);

                string message = $"You have been invited to the '{@event.Title}' event.";
                await _notificationHub.SendNotificationToClient(message, guest.UserName);
            }

            foreach (var guest in guestsRemoved)
            {
                string message = $"You have been removed from the '{@event.Title}' event.";
                await _notificationHub.SendNotificationToClient(message, guest.ApplicationUser.UserName);
            }

            var originalEvent = _context.Events.FirstOrDefault(e => e.Id == id);
            originalEvent.Title = @event.Title;
            originalEvent.Description = @event.Description;
            originalEvent.URL = @event.URL;
            originalEvent.Location = @event.Location;
            originalEvent.EventDate = @event.EventDate;
            originalEvent.CreatedBy = _userManager.GetUserId(User);
            _context.Events.Update(originalEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        public IActionResult SelectUsers(int eventId)
        {
            return RedirectToAction("SelectUsers", "Invitations", new { eventId = eventId });
        }

        public async Task<IActionResult> AcceptEvent(int eventId, string userId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            var guest = await _context.Users.FindAsync(userId);
            var eventGuest = await _context.EventGuests.Where(i => i.EventId == eventId && i.GuestId == userId).FirstOrDefaultAsync();

            if (@event == null || guest == null || eventGuest == null)
            {
                return NotFound();
            }

            eventGuest.Status = EApproval.Accepted;
            _context.EventGuests.Update(eventGuest);

            string creatorId = @event.CreatedBy;
            var creator = await _userManager.FindByIdAsync(creatorId);
            string creatorUsername = creator.UserName;
            string message = $"{guest.UserName} has accepted your invitation to the '{@event.Title}' event.";
            await _notificationHub.SendNotificationToClient(message, creatorUsername);

            var notification = new Notification
            {
                UserName = guest.UserName,
                Message = $"You have accepted the invitation to the '{@event.Title}' event.",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            string notificationJson = JsonConvert.SerializeObject(notification);
            await _notificationHub.SendNotificationToClient(notificationJson, guest.UserName);

            return RedirectToAction(nameof(GuestsSelect), new { eventId });
        }

        public async Task<IActionResult> DeclineEvent(int eventId, string userId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            var guest = await _context.Users.FindAsync(userId);
            var eventGuest = await _context.EventGuests.Where(i => i.EventId == eventId && i.GuestId == userId).FirstOrDefaultAsync();

            if (@event == null || guest == null || eventGuest == null)
            {
                return NotFound();
            }

            _context.EventGuests.Remove(eventGuest);

            string creatorId = @event.CreatedBy;
            var creator = await _userManager.FindByIdAsync(creatorId);
            string creatorUsername = creator.UserName;
            string message = $"{guest.UserName} has declined your invitation to the '{@event.Title}' event.";
            await _notificationHub.SendNotificationToClient(message, creatorUsername);

            var notification = new Notification
            {
                UserName = guest.UserName,
                Message = $"You have declined the invitation to the '{@event.Title}' event.",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            string notificationJson = JsonConvert.SerializeObject(notification);
            await _notificationHub.SendNotificationToClient(notificationJson, guest.UserName);

            return RedirectToAction(nameof(GuestsSelect), new { eventId });
        }

        public IActionResult GuestsSelect(int eventId)
        {
            var eventGuests = _context.EventGuests
                               .Include(e => e.ApplicationUser)
                               .Include(e => e.Event)
                               .Where(i => i.EventId == eventId);
            return View(eventGuests);
        }
    }
}