using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UserAPI.Validation
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return new ValidationResult("Phone number is required.");

            // Convert "00" to "+" if necessary.
            if (phoneNumber.StartsWith("00"))
            {
                phoneNumber = "+" + phoneNumber[2..];
            }

            // Add default country code if it's missing.
            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = "+46" + phoneNumber;
            }

            // Remove non-digit characters.
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // The length check is for 11 digits (9 + country code).
            if (digits.Length != 11)
                return new ValidationResult("Phone number must consist of 9 digits excluding the country code.");

            return ValidationResult.Success;
        }
    }
}
