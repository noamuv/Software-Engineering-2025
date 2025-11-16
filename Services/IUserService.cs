using Software_Engineering_2025.Models;

namespace Software_Engineering_2025.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(int userTypeId, string firstName, string lastName, string email, string password, string? title = null);
        Task<bool> DisableUserAsync(int userId);
        Task<bool> EnableUserAsync(int userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}