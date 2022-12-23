using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class AddMemberViewModel
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        [NotMapped]
        public List<string> Users { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
}
