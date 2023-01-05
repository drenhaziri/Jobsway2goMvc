using Jobsway2goMvc.Enums;

namespace Jobsway2goMvc.Models.ViewModel
{
    public class PostListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set;}
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public Approval Status { get; set; }
    }
}
