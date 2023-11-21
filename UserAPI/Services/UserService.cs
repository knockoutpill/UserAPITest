using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models;

namespace UserAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public UserService(UserDbContext context, IEmailService emailService, ISmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
            _passwordHasher = new PasswordHasher<User>();
        }

        /// <summary>
        /// Creates a new user with the given details.
        /// </summary>
        /// <param name="user">The user object to be created.</param>
        /// <returns>The created user with hashed password and formatted phone number.</returns>
        /// <remarks>
        /// This method performs the following:
        /// - If the phone number starts with '00', it converts '00' to '+'.
        /// - If the phone number does not start with '+', it adds '+46' as the default country code.
        /// - Hashes the user's password before saving.
        /// - Sends a notification to the user's email or phone number.
        /// </remarks>
        public async Task<User> CreateUserAsync(User user)
        {
            // Set ID to the highest ID + 1. Might get concurrency issues with this approach. Probably shouldn't send in the ID from the client at all
            if (_context.Users.Any())
            {
                // Find the highest ID and add 1
                int maxId = await _context.Users.MaxAsync(u => u.Id);
                user.Id = maxId + 1;
            }
            else
            {
                user.Id = 1; // Start with 1 if there are no users
            }

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

            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send notification. Don't actually ever send passwords over email or SMS.
            var message = $"Your account has been created. Email: {user.Email}, Phone: {user.Phone}, Password: {user.Password}";
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _emailService.SendEmailAsync(user.Email, "Account Created", message);
            }
            else if (!string.IsNullOrEmpty(user.Phone))
            {
                await _smsService.SendSmsAsync(user.Phone, message);
            }

            return user;
        }

        public async Task<bool> VerifyUserPasswordAsync(string email, string password)
        {
            // might want to do this also for phone number
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return verificationResult == PasswordVerificationResult.Success;
        }

        public async Task<User> FindUserByEmailOrPhoneAsync(string email, string phone)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.Phone == phone);
        }
    }
}
