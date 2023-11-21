using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using UserAPI.Models;
using UserAPI.Services;
using Moq;
using UserAPI.Data;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace UserAPITester
{
    public class UserServiceTests
    {
        [Fact]
        public void CreateUser_ShouldReturnUser_WhenValidInput()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateUser_ShouldReturnUser_WhenValidInput")
                .Options;

            // Use a using block to ensure the context is disposed after use, which is important for in-memory databases
            using (var context = new UserDbContext(options))
            {
                var mockEmailService = new Mock<IEmailService>();
                var mockSmsService = new Mock<ISmsService>();
                var userService = new UserService(context, mockEmailService.Object, mockSmsService.Object);

                var user = new User
                {
                    Email = "test@test.com",
                    Phone = "+46701234567",
                    Password = "Test1234!"
                };

                // Act
                var result = userService.CreateUserAsync(user).Result;

                // Assert
                result.Should().BeOfType<User>().And.NotBeNull("because a valid user should be created");
                result.Email.Should().Be(user.Email, "because the created user should have the same email as the input user");
                result.Phone.Should().Be(user.Phone, "because the created user should have the same phone number as the input user");
                result.Password.Should().NotBeNullOrEmpty("because the password should be set");
                result.Id.Should().BeGreaterThan(0, "because a new user should have a non-zero Id assigned");
            }
        }

        [Theory]
        [InlineData("123456789", "+46123456789")]      // No country code, should add +46
        [InlineData("00123456789", "+123456789")]      // Starts with 00, should convert to +
        public void CreateUser_ShouldFormatPhoneNumberCorrectly(string inputPhone, string expectedPhone)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbForPhoneNumberFormatting")
                .Options;

            using (var context = new UserDbContext(options))
            {
                var mockEmailService = new Mock<IEmailService>();
                var mockSmsService = new Mock<ISmsService>();
                var userService = new UserService(context, mockEmailService.Object, mockSmsService.Object);

                var user = new User
                {
                    Email = "test@test.com",
                    Phone = inputPhone,
                    Password = "mypwd"
                };

                // Act
                var result = userService.CreateUserAsync(user).Result;

                // Assert
                result.Phone.Should().Be(expectedPhone);
            }
        }

    }
}
