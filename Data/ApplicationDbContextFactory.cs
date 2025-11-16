using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Software_Engineering_2025.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            optionsBuilder.UseSqlite("Data Source=SensoreDB.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
