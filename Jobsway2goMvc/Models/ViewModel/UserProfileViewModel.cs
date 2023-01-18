namespace Jobsway2goMvc.Models.ViewModel
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public string? CompanyName { get; set; }
        public string? CompanyArea { get; set; }
        public string? Address { get; set; }
        public string? ImagePath { get; set; }
        public string? Certificates { get; set; }
        public string? Badges { get; set; }
        public string? Courses { get; set; }
        public string? References { get; set; }
        public Boolean? IsPremium { get; set; }
        public Boolean? IsActive { get; set; }
        public List<Experience> Experiences { get; set; }
    }
}
