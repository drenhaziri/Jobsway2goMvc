using NuGet.Packaging.Signing;

namespace Jobsway2goMvc.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public bool? IsRead { get; set; }
        public DateTime NotificationDateTime { get; set; }

    }
}
