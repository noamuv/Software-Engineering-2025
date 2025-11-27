using Software_Engineering_2025.Models;
using System.Linq;

namespace Software_Engineering_2025.Services
{
    // Service to handle authentication logic
    public class AuthenticationService
    {
        // Dependencies injected via constructor to support authentication operations
        private readonly ApplicationDbContext _context;
        private readonly PasswordValidator _passwordValidator;

    // Constructor to initialize dependencies
        public AuthenticationService(ApplicationDbContext context, PasswordValidator passwordValidator)
        {
            // Injected database context and password validator
            _context = context;
            _passwordValidator = passwordValidator;
        }

        // SIGN IN Authenticates user with temporary password
        public AuthenticationResult AuthenticateWithTemporaryPassword(string email, string temporaryPassword)
        {
            // Find user through their email in the database
            var user = _context.AppUsers.SingleOrDefault(u => u.Email == email);

            // If user not found, return failure
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

        // LOGIN - Authenticate with Regular Password (Returning Users)
         public AuthenticationResult AuthenticateWithPassword(string email, string password)
       {
         var user = _context.AppUsers.SingleOrDefault(u => u.Email == email);

           if (user == null)
            return AuthenticationResult.Fail("Invalid email or password.");

        // Check if user still needs to sign in for first time
           if (user.RequiresPasswordReset)
           return AuthenticationResult.Fail("Please use the 'First Time Sign In' option to set your password.");

         // Verify password
        if (string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(password, user.PasswordHash))
        return AuthenticationResult.Fail("Invalid email or password.");

       // Successful authentication
        return AuthenticationResult.Success(user);
       }

        // RESET user password
        public AuthenticationResult ResetPassword(Guid userId, string newPassword, string confirmPassword)
        {
            // Find user
            // Check the database for the user by their ID
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

            // Update user password and remove temporary password
            user.PasswordHash = HashPassword(newPassword);
            user.TemporaryPassword = null;
            user.RequiresPasswordReset = false;


            // Save changes to database
            _context.SaveChanges();

            return AuthenticationResult.Success(user);
        }

        // Hash password
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verify password hash (placeholder)
        private bool VerifyPassword(string password, string hash)
         {
            // BCrypt automatically handles the salt that's stored in the hash
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
     }

         // Result class for authentication operations
        public class AuthenticationResult
        {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        // Factory methods for success and failure results
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