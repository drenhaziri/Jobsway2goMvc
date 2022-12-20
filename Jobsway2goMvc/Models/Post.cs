using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public PostType Type { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}