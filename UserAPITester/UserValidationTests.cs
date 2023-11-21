using Xunit;
using FluentValidation;
using UserAPI.Models;
using UserAPITester.Helpers;
using UserAPI.Validation;

namespace UserAPITester
{
    public class UserValidationTests
    {
        private readonly UserValidator _validator = new UserValidator();
        private readonly PhoneNumberAttribute _phoneNumberValidator = new PhoneNumberAttribute();


        [Theory]
        [InlineData("invalid-email")]
        [InlineData("user@")]
        [InlineData("")]
        [InlineData(null)]
        public void CreateUser_InvalidEmail_ShouldFailValidation(string email)
        {
            // Arrange
            var user = new User 
            { 
                Email = email,
                Password = "mYPa$$w0Rd!!"
            };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void CreateUser_ValidEmail_ShouldPassValidation()
        {
            // Arrange
            var user = new User
            {
                Email = "validemail@test.com",
                Password = "mYPa$$w0Rd!!"
            };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("123456789", true)]       // Valid - 9 digits
        [InlineData("+46123456789", true)]    // Valid - 9 digits with country code
        [InlineData("0046123456789", true)]   // Valid - 9 digits with '00' prefix and country code
        [InlineData("12345678", false)]       // Invalid - Less than 9 digits
        [InlineData("1234567890", false)]     // Invalid - More than 9 digits
        [InlineData("abcdefghi", false)]      // Invalid - Non-digit characters
        [InlineData("", false)]               // Invalid - Empty string
        [InlineData(null, false)]             // Invalid - Null
        public void PhoneNumberValidation_ShouldValidateCorrectly(string phone, bool expectedIsValid)
        {
            // Arrange
            var user = new User 
            { 
                Phone = phone, 
                Password = "mYPa$$w0Rd!!"    
            };

            // convert phone number to correct format. Done outside the validation
            if (!string.IsNullOrEmpty(user.Phone))
            {
                // Convert '00' prefix to '+' in phone number
                if (user.Phone.StartsWith("00"))
                {
                    user.Phone = "+" + user.Phone[2..];
                }
                // Add default country code '+46' if no country code is present
                else if (!user.Phone.StartsWith("+"))
                {
                    user.Phone = "+46" + user.Phone;
                }
            }

            // Act
            var result = _phoneNumberValidator.IsValid(user.Phone);

            // Assert
            Assert.Equal(expectedIsValid, result);
        }
    }
}
