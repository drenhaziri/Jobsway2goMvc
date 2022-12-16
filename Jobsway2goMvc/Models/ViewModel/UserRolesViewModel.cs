using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class UserRolesViewModel 
    {
        [Key]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [NotMapped]
        public IEnumerable<string> Roles { get; set; }

    }
}
