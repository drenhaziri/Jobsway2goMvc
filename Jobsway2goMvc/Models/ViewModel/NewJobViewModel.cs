using Microsoft.AspNetCore.Mvc.Rendering;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class NewJobViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string Schedule { get; set; }
        public string Description { get; set; }
        public int OpenSpots { get; set; }
        public string Requirements { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Payment { get; set; }
        //public List<User> Applicants { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
