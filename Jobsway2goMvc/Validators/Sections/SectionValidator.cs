using FluentValidation;
using Jobsway2goMvc.Models;

namespace Jobsway2goMvc.Validators.Sections;

public class SectionValidator : AbstractValidator<Section>
{
    public SectionValidator()
    {
        RuleFor(e => e.Name).NotEmpty().WithMessage("Please specify a name");
        //RuleFor(e => e.Description).NotEmpty();
        RuleFor(e => e.Location).NotEmpty().WithMessage("Please specify a location");
        RuleFor(e => e.DateFrom).NotEmpty().Must(ValidDateFrom).WithMessage("Please specify a date from");
        RuleFor(e => e.DateTo).NotEmpty().WithMessage("Please specify a date to").WithMessage("Please specify a Valid Date");
    }

    protected bool ValidDateFrom(DateTime fromDate)
    {
        DateTime currentDate = DateTime.Now;
        return fromDate <= currentDate;
    }

}
