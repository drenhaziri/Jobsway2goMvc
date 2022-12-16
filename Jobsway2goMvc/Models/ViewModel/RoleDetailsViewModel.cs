using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class RoleDetailsViewModel
    {
        [Key]
        public string Id { get; set; }
        public string RoleName { get; set; }

        [NotMapped]
        public List<string> Users { get; set; } = new List<string>();

    }
}
