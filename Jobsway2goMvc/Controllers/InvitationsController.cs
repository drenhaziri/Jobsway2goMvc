using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Hubs;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Jobsway2goMvc.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationHub _notificationHub;

        public InvitationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, NotificationHub notificationHub)
        {
            _context = context;
            _userManager = userManager;
            _notificationHub = notificationHub;
        }

        public async Task<IActionResult> SelectUsers(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
            {
                return NotFound();
            }

            var users = await _context.Users.ToListAsync();
            var invitations = await _context.Invitations.Where(i => i.EventId == eventId).ToListAsync();

            var model = new InvitationEventViewModel
            {
                Users = users,
                Invitations = invitations,
                UserManager = _userManager,
                Context = _context,
                EventId = eventId
            };

            ViewBag.EventCreatedBy = @event.CreatedBy;
            ViewBag.HttpContextUser = HttpContext.User;

            return View(model);
        }

        public async Task<IActionResult> SendInvite(int eventId, string userId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (@event == null || user == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId != @event.CreatedBy)
            {
                return Unauthorized();
            }

            var invitation = new Invitation
            {
                Event = @event,
                User = user,
                Status = EApproval.Pending
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            await CreateInvitationNotification(eventId, userId);

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }

        public async Task<IActionResult> CancelInvite(int eventId, string userId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (@event == null || user == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(HttpContext.User);

            if (currentUserId != @event.CreatedBy)
            {
                return Unauthorized();
            }

            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.EventId == eventId && i.UserId == userId);

            if (invitation == null)
            {
                return NotFound();
            }

            var notification = new Notification
            {
                UserName = user.UserName,
                Message = $"Your invitation to the '{@event.Title}' event has been canceled.",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();

            string notificationJson = JsonConvert.SerializeObject(notification);
            await _notificationHub.SendNotificationToClient(notificationJson, user.UserName);

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }

        public async Task<IActionResult> AcceptInvite(int eventId, string userId)
        {
            var invitation = await _context.Invitations.Where(i => i.EventId == eventId && i.UserId == userId).FirstOrDefaultAsync();
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (invitation == null || @event == null || user == null)
            {
                return NotFound();
            }

            invitation.Status = EApproval.Accepted;
            _context.Invitations.Update(invitation);

            string creatorId = @event.CreatedBy;
            var creator = await _userManager.FindByIdAsync(creatorId);
            string creatorUsername = creator.UserName;
            string message = $"{user.UserName} has accepted your invitation to the '{@event.Title}' event.";
            await _notificationHub.SendNotificationToClient(message, creatorUsername);

            var notification = new Notification
            {
                UserName = user.UserName,
                Message = $"You have accepted the invitation to the '{@event.Title}' event.",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            string notificationJson = JsonConvert.SerializeObject(notification);
            await _notificationHub.SendNotificationToClient(notificationJson, user.UserName);

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }


        public async Task<IActionResult> DeclineInvite(int eventId, string userId)
        {
            var invitation = await _context.Invitations.Where(i => i.EventId == eventId && i.UserId == userId).FirstOrDefaultAsync();
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (invitation == null || @event == null || user == null)
            {
                return NotFound();
            }

            _context.Invitations.Remove(invitation);

            string creatorId = @event.CreatedBy;
            var creator = await _userManager.FindByIdAsync(creatorId);
            string creatorUsername = creator.UserName;
            string message = $"{user.UserName} has declined your invitation to the '{@event.Title}' event.";
            await _notificationHub.SendNotificationToClient(message, creatorUsername);

            var notification = new Notification
            {
                UserName = user.UserName,
                Message = $"You have declined the invitation to the '{@event.Title}' event.",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            string notificationJson = JsonConvert.SerializeObject(notification);
            await _notificationHub.SendNotificationToClient(notificationJson, user.UserName);

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }

        public async Task CreateInvitationNotification(int eventId, string userId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (@event == null || user == null)
            {
                return;
            }

            var notification = new Notification
            {
                UserName = user.UserName,
                Message = $"You have been invited to the '{@event.Title}' event!",
                MessageType = "Personal",
                NotificationDateTime = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _notificationHub.SendNotificationToClient(notification.Message, notification.UserName);
        }

    }
}