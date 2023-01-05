using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Models;
using Jobsway2goMvc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvitationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }

        public async Task<IActionResult> AcceptInvite(int eventId, string userId)
        {
            var invitation = await _context.Invitations.Where(i => i.EventId == eventId && i.UserId == userId).FirstOrDefaultAsync();

            if (invitation == null)
            {
                return NotFound();
            }

            invitation.Status = EApproval.Accepted;
            _context.Invitations.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }

        public async Task<IActionResult> DeclineInvite(int eventId, string userId)
        {
            var invitation = await _context.Invitations.Where(i => i.EventId == eventId && i.UserId == userId).FirstOrDefaultAsync();

            if (invitation == null)
            {
                return NotFound();
            }

            invitation.Status = EApproval.Rejected;
            _context.Invitations.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SelectUsers), new { eventId });
        }
    }
}