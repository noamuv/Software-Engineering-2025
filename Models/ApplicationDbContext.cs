using Microsoft.EntityFrameworkCore;

namespace Software_Engineering_2025.Models
{
    public class ApplicationDbContext : DbContext
    {
        //Constructor to configure the database context
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //This Dbset represents the AppUser table in the database
        public DbSet<AppUser> AppUsers { get; set; }
    }
}