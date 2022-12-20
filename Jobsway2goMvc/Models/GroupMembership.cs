namespace Jobsway2goMvc.Models
{
    public class GroupMembership
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Group Group { get; set; }
    }
}
