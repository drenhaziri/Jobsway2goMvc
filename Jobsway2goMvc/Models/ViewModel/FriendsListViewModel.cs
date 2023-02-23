using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class FriendsListViewModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ConnectionStatus Status { get; set; }
    }
}
