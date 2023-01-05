using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Microsoft.AspNetCore.Identity;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class InvitationEventViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Invitation> Invitations { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ApplicationDbContext Context { get; set; }
        public EApproval? Status { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}