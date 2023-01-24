using Microsoft.AspNetCore.Identity;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class EventViewModel
    {
        public Event Event { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public List<EventGuest> EventGuests { get; set; }
    }
}
