using FluentValidation;
using Jobsway2goMvc.Enums;
using Jobsway2goMvc.Models;

namespace Jobsway2goMvc.Validators.Jobs
{
    public class JobValidator : AbstractValidator<Job>
    {
        public JobValidator()
        {
            RuleFor(e => e.CompanyName).NotEmpty().WithMessage("Please specify a company name");
            RuleFor(e => e.Location).Must((e, location) => {
                if (location == JobLocation.None)
                    return false;

                return true;
            }).WithMessage("Please specify a location");
            RuleFor(e => e.Schedule).Must((e, position) => {
                if (position == JobPosition.None)
                    return false;

                return true;
            }).WithMessage("Please specify a schedule");
            RuleFor(e => e.Site).Must((e, site) => {
                if (site == JobSite.None)
                    return false;

                return true;
            }).WithMessage("Please specify a site");
            RuleFor(e => e.Description).NotEmpty().WithMessage("Please specify a description");
            RuleFor(e => e.OpenSpots).NotEmpty().WithMessage("Please specify an OpenSpots");
            RuleFor(e => e.Requirements).NotEmpty().WithMessage("Please specify Requirements");
            RuleFor(e => e.DateFrom).NotEmpty().WithMessage("Please enter  DateFrom");
            RuleFor(e => e.DateTo).NotEmpty().WithMessage("Please enter  DateTo");
            RuleFor(e => e.MinSalary).NotEmpty().WithMessage("Please enter  MinSalary");
            RuleFor(e => e.MaxSalary).NotEmpty().WithMessage("Please enter  MaxSalary");
            RuleFor(e => e.CategoryId).NotEmpty().WithMessage("Please specify a Category");
            RuleFor(x => x.MinSalary)
           .GreaterThanOrEqualTo(1)
           .WithMessage("Min. number of characters for citation must be greater than or equal to 1")
           .LessThanOrEqualTo(x => x.MaxSalary)
           .WithMessage("Min. number of characters must be less than or equal to max. number of characters");
            RuleFor(x => x.MaxSalary)
           .GreaterThanOrEqualTo(1)
           .WithMessage("Max. number of characters for citation must be greater than or equal to 1");
            RuleFor(x => x.DateFrom)
           .LessThanOrEqualTo(x => x.DateTo)
           .WithMessage("DateFrom number of characters must be less than or equal to DateTo.");
        }
    }
}
