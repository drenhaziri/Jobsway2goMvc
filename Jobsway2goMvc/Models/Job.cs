namespace Jobsway2goMvc.Models
{
    public class Job
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
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        //public List<User> Applicants { get; set; }
        public int CategoryId { get; set; } 
        public JobCategory Category { get; set; }

    }
}
