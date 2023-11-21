using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserAPI.Validation
{
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult("Password is required.");

            var password = value.ToString();

            if (password.Length < 8)
                return new ValidationResult("Password must be at least 8 characters long.");

            if (!password.Any(char.IsLower))
                return new ValidationResult("Password should contain at least one lowercase character.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("Password should contain at least one uppercase character.");

            if (password.Count(char.IsDigit) < 2)
                return new ValidationResult("Password should contain at least two digits.");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
                return new ValidationResult("Password should contain at least one symbol.");

            return ValidationResult.Success!;
        }
    }
}
