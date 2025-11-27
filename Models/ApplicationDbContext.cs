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

        //Configures the model properties and relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AppUser entity
            modelBuilder.Entity<AppUser>(entity =>
            {
                // Set primary key and property constraints
                // This ensures data integrity and enforces rules at the database level
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

                // This allows PasswordHash and TemporaryPassword to be nullable
                entity.Property(e => e.PasswordHash).IsRequired(false);
                entity.Property(e => e.TemporaryPassword).IsRequired(false);
            });
        }
    }
}