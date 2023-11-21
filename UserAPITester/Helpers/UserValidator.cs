using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Models;

namespace UserAPITester.Helpers
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address format.")
            .MaximumLength(256);
            RuleFor(user => user.Phone)
            .Must(BeAValidPhoneNumber)
            .WithMessage("Phone number must consist of 9 digits excluding the country code.");

        }
        private bool BeAValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return true; // phone number is optional

            var digits = phoneNumber.Where(char.IsDigit).ToArray();
            return digits.Length == 9;
        }
    }
}
