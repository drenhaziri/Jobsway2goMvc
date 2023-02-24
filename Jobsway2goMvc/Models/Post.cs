using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new List<Comment>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAtUTC { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? ImagePath { get; set; }
        public string? CreatedById { get; set; }
        public PostType Type { get; set; }
        public Approval Status { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}