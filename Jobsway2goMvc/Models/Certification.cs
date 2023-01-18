namespace Jobsway2goMvc.Models;

public class Certification : Section
{
    public string InstitutionName { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}