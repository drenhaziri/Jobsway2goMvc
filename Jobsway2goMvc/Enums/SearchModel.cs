using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Enums
{
    public enum SearchModel
    {
        [Display(Name = "Select Type")]
        SelectType,
        User,
        Job,
        Event,
        Group
    }
}
