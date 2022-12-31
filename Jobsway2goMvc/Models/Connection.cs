using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models
{
    public class Connection
    {
        public int Id { get; set; } 
        public string Connect1 { get; set; } //User current Id
        public string Connect2 { get; set; } //User id from details  
        public ConnectionStatus Status { get;set; }
    }
}
