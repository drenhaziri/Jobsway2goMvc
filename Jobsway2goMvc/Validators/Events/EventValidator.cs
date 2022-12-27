using FluentValidation;
using Jobsway2goMvc.Models;

namespace Jobsway2goMvc.Validators.Events;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(e => e.Title).NotEmpty();
        RuleFor(e => e.Description).NotEmpty();
        RuleFor(e => e.CompanyName).NotEmpty().WithMessage("Please specify a company name");
        RuleFor(e => e.ImagePath).NotEmpty().WithMessage("Please upload a image");
        //RuleFor(e => e.Speakers).NotEmpty().WithMessage(" Please Choose  speaker ");
        RuleFor(e => e.URL).NotEmpty().WithMessage("Please add a link");
        RuleFor(e => e.Location).NotEmpty().WithMessage("Please specify a Location");
    }
}
