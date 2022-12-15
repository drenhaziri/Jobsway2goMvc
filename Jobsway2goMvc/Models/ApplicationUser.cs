using Microsoft.AspNetCore.Identity;

namespace Jobsway2goMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyArea { get; set; }
        public string? Address { get; set; }
        public string? ImagePath { get; set; }
        public string? Certificates { get; set; }

        /*public List<Collection> Collections {get;set;} 
         * public List<Language> Languages { get; set; }
        public List<Project> Projects { get; set; }*/

        public string? Badges { get; set; }
        public string? Courses { get; set; }
        public string? References { get; set; }
        public Boolean? IsPremium { get; set; }
        public Boolean? IsActive { get; set; }

    }
}
