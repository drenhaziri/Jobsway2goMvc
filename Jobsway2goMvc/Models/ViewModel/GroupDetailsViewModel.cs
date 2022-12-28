using Jobsway2goMvc.Data;
using Jobsway2goMvc.Enums;
using Microsoft.AspNetCore.Identity;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class GroupDetailsViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<GroupMembership> GroupMemberships { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public ApplicationDbContext Context { get; set; }
        public Approval? Status { get; set; }
        public int GroupId { get; set; }
    }
}
