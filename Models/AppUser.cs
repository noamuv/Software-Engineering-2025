using System;

namespace Software_Engineering_2025.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }

        public required string Email { get; set; }

        
        public required string? FirstName { get; set; } 
        public required string? LastName { get; set; } 

        //Admin sends a temporary password for first login
        public string TemporaryPassword { get; set; } = null;
         
        //New users must reset password on first login
        public bool RequiresPasswordReset { get; set; } = true;

        //After reset, store hashed password
        public string PasswordHash { get; set; } = null;


        public string Role { get; set; } = null; // e.g., "Admin", "User", etc.


    }
}