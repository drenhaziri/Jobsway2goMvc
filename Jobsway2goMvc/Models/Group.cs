using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? CreatedBy { get; set; }
/*
        [NotMapped]
        public List<ApplicationUser> Members { get; set; }
        public bool IsMember { get; set; }*/

        [NotMapped]
        public ICollection<Post> Posts { get; set; }

    }
}
