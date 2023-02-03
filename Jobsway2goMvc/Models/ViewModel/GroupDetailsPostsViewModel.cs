namespace Jobsway2goMvc.Models.ViewModel
{
    public class GroupDetailsPostsViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CreatedBy { get; set; }  
        public ICollection<Post>? Posts { get; set; }
        public Boolean IsPublic { get; set; }
        public string? Description { get;set; }
        public GroupMembership? CurrentMembershipList { get; set; }
    }
}
