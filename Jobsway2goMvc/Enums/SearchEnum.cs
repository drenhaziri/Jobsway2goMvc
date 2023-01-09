using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Enums
{
    public enum SearchEnum
    {
        [Display(Name = "Select Type")]
        SelectType,
        Business,
        User,
        Job,
        Event,
        Group
    }
}
