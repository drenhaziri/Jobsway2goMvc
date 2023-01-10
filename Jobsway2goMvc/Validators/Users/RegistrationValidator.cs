using FluentValidation;
using Jobsway2goMvc.Areas.Identity.Pages.Account;

namespace Jobsway2goMvc.Validators.Users
{
    public class RegistrationValidator: AbstractValidator<RegisterModel.InputModel>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("FirstName should contain only letters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("LastName should contain only letters.");
        }
        private bool BeValidName(string name)
        {
            return name.All(Char.IsLetter);
        }
    }
}
