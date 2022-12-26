using FluentValidation;
using Jobsway2goMvc.Models;

namespace Jobsway2goMvc.Validators.Job_Category
{
    public class JobCategoryValidator : AbstractValidator<JobCategory>
    {
        public JobCategoryValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Please specify a job category name"); ;
            RuleFor(e => e.Jobs).NotEmpty();
        }
    }
}
