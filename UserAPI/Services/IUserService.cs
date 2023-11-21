using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<User> FindUserByEmailOrPhoneAsync(string email, string phone);
        Task<bool> VerifyUserPasswordAsync(string email, string password);
    }
}
