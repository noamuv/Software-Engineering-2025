
// AppUser.cs
// Defines the AppUser model representing users in the application.

using System;

namespace Software_Engineering_2025.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }
        
        // User's email address, used for login
        public required string Email { get; set; }
       // Admin adds first and last name for user profile
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 

        /* The pressure data given by Sensore 
          is titled with a unique CSV user ID and dates for user identification.
            This ID is linked to the AppUser for data association
            */
        public string? CsvUserId { get; set; }
        //Admin sends a temporary password for first login
        public string TemporaryPassword { get; set; } = null;
        //New users must reset password on first login
        public bool RequiresPasswordReset { get; set; } = true;
        //After reset, store hashed password
        public string PasswordHash { get; set; } = null;
        public string Role { get; set; } = null; // e.g., "Admin", "User", etc.
    }
}