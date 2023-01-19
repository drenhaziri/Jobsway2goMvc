using FluentValidation;
using Jobsway2goMvc.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Jobsway2goMvc.Validators.Users
{
    public class RegistrationValidator : AbstractValidator<RegisterModel.InputModel>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("First Name should contain only letters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("This field is required.")
                .Must(BeValidName).WithMessage("Last Name should contain only letters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("This field is required.")
                .Must(ValidateEmail).WithMessage("The email format is not valid");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("This field is required.")
                .MinimumLength(6).WithMessage("The password must be at least 6 characters long.")
                .Must(IsValidPassword).WithMessage("The password should must contain at least one uppercase and lowercase letter at least one number and at least non alphanumeric character");
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("This field is required.")
                .Custom((confirmPassword, context) => {
                                                         var password = context.InstanceToValidate.Password;
                                                        if (confirmPassword != password)
                                                        {
                                                            context.AddFailure("The password and confirmation password do not match.");
                                                        }
                                                             });
        }
        private bool BeValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            var regex = new Regex(@"^[a-zA-Z]+$");
            return regex.IsMatch(name);
        }
        private bool ValidateEmail(string email)

        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            var regex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            bool result = Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
            return result;
        }
        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            return Regex.IsMatch(password, pattern);

        }
    }
}