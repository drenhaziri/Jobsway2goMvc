using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class EventGuest
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public string GuestId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public EApproval Status { get; set; }
    }
}