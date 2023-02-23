using Jobsway2goMvc.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public List<string> GuestsIds { get; set; }
        public virtual ICollection<EventGuest> EventGuests { get; set; }
        public EApproval Status { get; set; }
        public string URL { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public string? CreatedBy { get; set; }
        public virtual ApplicationUser CreatedByName { get; set; }
        public string CreatorName
        {
            get
            {
                return CreatedByName != null ? $"{CreatedByName.FirstName} {CreatedByName.LastName}" : null;
            }
        }

    }
}