using FluentValidation;
using Jobsway2goMvc.Models;

namespace Jobsway2goMvc.Validators.Jobs
{
    public class JobValidator : AbstractValidator<Job>
    {
        public JobValidator()
        {
            RuleFor(e => e.CompanyName).NotEmpty().WithMessage("Please specify a company name");
            RuleFor(e => e.Location).NotEmpty().WithMessage("Please specify a location");
            RuleFor(e => e.Schedule).NotEmpty().WithMessage("Please specify a Schedule");
            RuleFor(e => e.Description).NotEmpty().WithMessage("Please specify a description");
            RuleFor(e => e.OpenSpots).NotEmpty().WithMessage("Please specify an OpenSpots");
            RuleFor(e => e.Requirements).NotEmpty().WithMessage("Please specify Requirements");
            RuleFor(e => e.DateFrom).NotEmpty().WithMessage("Please enter  DateFrom");
            RuleFor(e => e.DateTo).NotEmpty().WithMessage("Please enter  DateTo");
            RuleFor(e => e.MinSalary).NotEmpty().WithMessage("Please enter  MinSalary");
            RuleFor(e => e.MaxSalary).NotEmpty().WithMessage("Please enter  MaxSalary");
            RuleFor(e => e.CategoryId).NotEmpty().WithMessage("Please specify a Category");
        }
    }
}
