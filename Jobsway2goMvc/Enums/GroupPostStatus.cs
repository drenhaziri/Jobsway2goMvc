using System.ComponentModel.DataAnnotations;

namespace Jobsway2goMvc.Enums
{
    public enum GroupPostStatus
    {
        [Display(Name = "Post Status")]
        Pending,
        Accepted,
        Rejected
    }
}