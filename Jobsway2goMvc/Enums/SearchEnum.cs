using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Enums
{
    public enum SearchEnum
    {
        [Display(Name = "Select Type")]
        SelectType,
        User,
        Job,
        Event,
        Group
    }
}
