using FluentValidation;
using Jobsway2goMvc.Models;
using System.Runtime.CompilerServices;

namespace Jobsway2goMvc.Validators.Collections;

public class CollectionValidator : AbstractValidator<Collection>
{
    public CollectionValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("The collection must have a name");
        RuleFor(c => c.User).NotEmpty();
    }
}