using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? CreatedBy { get; set; }
        public ICollection<Post> Posts { get; set; }
        public Boolean IsPublic { get; set; } = true;

    }
}
