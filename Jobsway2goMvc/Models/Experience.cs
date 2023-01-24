namespace Jobsway2goMvc.Models;

public class Experience : Section
{
    public string Description { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}