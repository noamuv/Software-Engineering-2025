namespace Software_Engineering_2025.Services
{
    public class PasswordValidator
    {
        // Validates password strength
        public ValidationResult ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return ValidationResult.Fail("Password cannot be empty.");

            if (password.Length < 8)
                return ValidationResult.Fail("Password must be at least 8 characters long.");

            if (!password.Any(char.IsUpper))
                return ValidationResult.Fail("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                return ValidationResult.Fail("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                return ValidationResult.Fail("Password must contain at least one number.");

            if (!password.Any(ch => "!@#$%^&*()_+-=[]{};:'\",.<>/?".Contains(ch)))
                return ValidationResult.Fail("Password must contain at least one special character.");

            return ValidationResult.Success();
        }

        // Validates that passwords match
        public ValidationResult ValidatePasswordsMatch(string password, string confirmPassword)
        {
            if (password != confirmPassword)
                return ValidationResult.Fail("Passwords do not match.");

            return ValidationResult.Success();
        }
    }

    // Simple result class to hold validation outcome
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        
        public static ValidationResult Fail(string message) => new ValidationResult 
        { 
            IsValid = false, 
            ErrorMessage = message 
        };
    }
}