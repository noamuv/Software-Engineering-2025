using Software_Engineering_2025.Models;
using System;
using System.Linq;

// Seed initial data into the database
namespace Software_Engineering_2025.Data
{
    // Static class to handle database seeding
    public static class DatabaseSeeder
    {
        // Method to seed initial users into the database
        public static void Seed(ApplicationDbContext context)
        {
            // If any users already exist, don't reseed
            if (context.AppUsers.Any())
                return;

            var users = new[]
            {
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "patient1@example.com",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Patient"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "clinician1@example.com",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Clinician"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "carer1@example.com",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Carer"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "admin1@example.com",
                    TemporaryPassword = "AdminPass123!",
                    RequiresPasswordReset = true,
                    Role = "Admin"
                }
            };

            context.AppUsers.AddRange(users);
            context.SaveChanges();
        }
    }
}