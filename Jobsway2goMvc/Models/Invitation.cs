using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public EApproval Status { get; set; }
    }
}