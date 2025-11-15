using System;

namespace Software_Engineering_2025.Models
{
    public class AppUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; } 

        //Admin sends a temporary password for first login
        public string TemporaryPassword { get; set; }
         
        //New users must reset password on first login
        public bool RequiresPasswordReset { get; set; } = true;

        //After reset, store hashed password
        public string PasswordHash { get; set; }

        public string Role { get; set; } // e.g., "Admin", "User", etc.


    }
}