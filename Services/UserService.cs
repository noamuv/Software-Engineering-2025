using Microsoft.EntityFrameworkCore;
using Software_Engineering_2025.Data;
using Software_Engineering_2025.Models;

namespace Software_Engineering_2025.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.User_ID == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserType)
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(int userTypeId, string firstName, string lastName, string email, string password, string? title = null)
        {
            // Hashes the password
            string passwordHash = _passwordService.HashPassword(password);

            // Create user
            var user = new User
            {
                User_Type_ID = userTypeId,
                Title = title,
                First_Name = firstName,
                Last_Name = lastName,
                Email = email,
                Password_Hash = passwordHash,
                Status = "Active",
                Created_At = DateTime.Now,
                Is_Activated = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DisableUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = "Disabled";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EnableUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = "Active";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}