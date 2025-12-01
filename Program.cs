using Microsoft.EntityFrameworkCore;
using Software_Engineering_2025.Models;
using Software_Engineering_2025.Data;
using Software_Engineering_2025.Services;  


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure SQLServer Database  
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dependency injection for services
builder.Services.AddScoped<PasswordValidator>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<PressureDataService>();
builder.Services.AddScoped<CsvImportService>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var  csvImport = scope.ServiceProvider.GetRequiredService<CsvImportService>();
    
    // Apply any pending migrations
    Console.WriteLine("Applying migrations...");
    context.Database.Migrate();
    
    // Seed initial data
    Console.WriteLine("Running seeder...");
    DatabaseSeeder.Seed(context);
}

   if (!context.PressureSessions.Any())
        {
            Console.WriteLine("📊 Importing pressure data from CSV files...");
            
            // Get path to CSV folder in project
            var csvFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "CsvFiles");
            
            if (Directory.Exists(csvFolderPath))
            {
                csvImport.ImportCsvFolder(csvFolderPath);
                Console.WriteLine("✅ Pressure data imported successfully!");
            }
            else
            {
                Console.WriteLine("⚠️  CSV folder not found. Please add CSV files to Data/CsvFiles/");
            }
        }
        else
        {
            Console.WriteLine("✓ Pressure data already exists in database");
        }
    
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during initialization: {ex.Message}");
    }


Console.WriteLine("🚀 Application ready!");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();   
app.UseAuthorization();


//app.MapStaticAssets();  commented out because of error as i have migrated to .net 8

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Index}/{id?}");

    // .WithStaticAssets(); commented out because of error as i have migrated to .net 8


app.Run();
