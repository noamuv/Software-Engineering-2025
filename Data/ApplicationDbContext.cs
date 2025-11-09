using Microsoft.EntityFrameworkCore;
using Software_Engineering_2025.Models;

namespace Software_Engineering_2025.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Define database tables
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Clinician> Clinicians { get; set; }
        public DbSet<Gender> Genders { get; set; }

        // Need to add when teammates provide models:
        // public DbSet<PressureSession> PressureSessions { get; set; }
        // public DbSet<Frame> Frames { get; set; }
        // public DbSet<Metric> Metrics { get; set; }
        // public DbSet<Alert> Alerts { get; set; }
        // public DbSet<Report> Reports { get; set; }
        // public DbSet<Comment> Comments { get; set; }
        // public DbSet<MovementPlan> MovementPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-UserType relationship (One-to-Many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.User_Type_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Admin-User relationship (One-to-One)
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Admin>(a => a.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Patient-Clinician relationship (Many-to-One)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Clinician)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.Clinician_User_ID)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Patient-Gender relationship (Many-to-One)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Gender)
                .WithMany(g => g.Patients)
                .HasForeignKey(p => p.Gender_ID)
                .OnDelete(DeleteBehavior.SetNull);

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed initial data for UserType lookup table
            modelBuilder.Entity<UserType>().HasData(
                new UserType 
                { 
                    User_Type_ID = 1, 
                    Type_Name = "Admin", 
                    Description = "System Administrator with full access" 
                },
                new UserType 
                { 
                    User_Type_ID = 2, 
                    Type_Name = "Clinician", 
                    Description = "Healthcare professional managing patients" 
                },
                new UserType 
                { 
                    User_Type_ID = 3, 
                    Type_Name = "Patient", 
                    Description = "Patient user monitoring pressure data" 
                }
            );

            // Seed initial data for Gender lookup table
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Gender_ID = 1, Name = "Male" },
                new Gender { Gender_ID = 2, Name = "Female" },
                new Gender { Gender_ID = 3, Name = "Other" },
                new Gender { Gender_ID = 4, Name = "Prefer not to say" }
            );
        }
    }
}