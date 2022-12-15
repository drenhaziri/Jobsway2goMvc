namespace Jobsway2goMvc.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationUser> Users { get; set; }
        //public List<AdminUser> Admin {get;set;}
        //public List<ModeratoUser> Moderator {get;set;}
    }
}
