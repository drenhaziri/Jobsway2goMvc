using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Enums
{
    public enum PostType
    {
        [Display(Name = "Select Type")]
        SelectType,
        Standard,
        Premium
    }
}