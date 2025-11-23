using Software_Engineering_2025.Models;
using System.Linq;

namespace Software_Engineering_2025.Services
{
    public class AuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordValidator _passwordValidator;

        public AuthenticationService(ApplicationDbContext context, PasswordValidator passwordValidator)
        {
            _context = context;
            _passwordValidator = passwordValidator;
        }

        // Authenticates user with temporary password
        public AuthenticationResult AuthenticateWithTemporaryPassword(string email, string temporaryPassword)
        {
            // Find user
            var user = _context.AppUsers.SingleOrDefault(u => u.Email == email);

            if (user == null)
                return AuthenticationResult.Fail("Account was not found.");

            // Check if user needs password reset
            if (!user.RequiresPasswordReset)
                return AuthenticationResult.Fail("Please use your regular password to log in.");

            // Validate temporary password
            if (user.TemporaryPassword != temporaryPassword)
                return AuthenticationResult.Fail("Invalid temporary password.");

            return AuthenticationResult.Success(user);
        }

        // Resets user password
        public AuthenticationResult ResetPassword(Guid userId, string newPassword, string confirmPassword)
        {
            // Find user
            var user = _context.AppUsers.Find(userId);
            if (user == null)
                return AuthenticationResult.Fail("User not found.");

            // Validate passwords match
            var matchResult = _passwordValidator.ValidatePasswordsMatch(newPassword, confirmPassword);
            if (!matchResult.IsValid)
                return AuthenticationResult.Fail(matchResult.ErrorMessage);

            // Validate password strength
            var strengthResult = _passwordValidator.ValidatePasswordStrength(newPassword);
            if (!strengthResult.IsValid)
                return AuthenticationResult.Fail(strengthResult.ErrorMessage);

            // Update user password (TODO: Hash this!)
            user.PasswordHash = newPassword;
            user.TemporaryPassword = null;
            user.RequiresPasswordReset = false;

            _context.SaveChanges();

            return AuthenticationResult.Success(user);
        }

        // Hash password (placeholder - implement properly later)
        private string HashPassword(string password)
        {
            // TODO: Use BCrypt.Net-Next
            // return BCrypt.Net.BCrypt.HashPassword(password);
            return password; // Temporary
        }

        // Verify password hash (placeholder)
        private bool VerifyPassword(string password, string hash)
        {
            // TODO: Use BCrypt.Net-Next
            // return BCrypt.Net.BCrypt.Verify(password, hash);
            return password == hash; // Temporary
        }
    }

    // Result class for authentication operations
    public class AuthenticationResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public static AuthenticationResult Success(AppUser user) => new AuthenticationResult
        {
            IsSuccessful = true,
            User = user
        };

        public static AuthenticationResult Fail(string message) => new AuthenticationResult
        {
            IsSuccessful = false,
            ErrorMessage = message
        };
    }
}