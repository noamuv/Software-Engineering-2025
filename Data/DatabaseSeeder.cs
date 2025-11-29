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
            // This prevents duplicate entries on multiple runs
            if (context.AppUsers.Any())
                return;


            // Create initial users with temporary passwords
            var users = new[]
            {
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "patient1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                     CsvUserId = "71e66ab3",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Patient"
                },
                 new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "qaedasalawu@gmail.com",
                    FirstName = "Qaeda",
                    LastName = "Salawu",
                    CsvUserId = "1c0fd777",
                    TemporaryPassword = "TestPass123!",
                    RequiresPasswordReset = true,
                    Role = "Patient"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "clinician1@example.com",
                    FirstName = "Alice",
                    LastName = "Smith",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Clinician"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "drmahmoudawad@barnethospital.co.uk",
                    FirstName = "Mahmoud",
                    LastName = "Awad",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Clinician"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "carer1@example.com",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Carer"
                },
                 new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "noamuretavidal@gmail.com",
                    FirstName = "Noam",
                    LastName = "Ureta Vidal",
                    TemporaryPassword = "TempPass123!",
                    RequiresPasswordReset = true,
                    Role = "Carer"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "admin1@example.com",
                    FirstName = "Robert",
                    LastName = "King",
                    TemporaryPassword = "AdminPass123!",
                    RequiresPasswordReset = true,
                    Role = "Admin"
                },
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = "nailamayanja@admin.com",
                    FirstName = "Naila",
                    LastName = "Mayanja",
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