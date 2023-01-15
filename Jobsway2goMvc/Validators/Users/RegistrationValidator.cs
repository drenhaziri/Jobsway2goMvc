using FluentValidation;
using Jobsway2goMvc.Areas.Identity.Pages.Account;
using System.Text.RegularExpressions;

namespace Jobsway2goMvc.Validators.Users
{
    public class RegistrationValidator : AbstractValidator<RegisterModel.InputModel>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("FirstName should contain only letters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("LastName should contain only letters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("This field is required.")
                .Must(ValidateEmail).WithMessage("The email format is not valid");
        }
        private bool BeValidName(string name)
        {
            var regex = new Regex(@"^[a-zA-Z]+$");
            return regex.IsMatch(name);
        }
        private static bool ValidateEmail(string email)
        {
            var regex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            bool result = Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
            return result;
        }
    }
}
