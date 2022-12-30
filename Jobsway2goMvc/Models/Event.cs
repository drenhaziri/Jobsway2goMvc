namespace Jobsway2goMvc.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string ImagePath { get; set; }
        public List<ApplicationUser> Speakers { get; set; }
        public string URL { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
    }
}
