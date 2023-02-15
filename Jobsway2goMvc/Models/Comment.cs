namespace Jobsway2goMvc.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get;set; }
        public string? UserImage { get; set; }

        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
