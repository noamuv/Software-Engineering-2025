using Microsoft.EntityFrameworkCore;
using Software_Engineering_2025.Models;
using Software_Engineering_2025.Data;
using Software_Engineering_2025.Services;  


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure SQLite Database  
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dependency injection for services
builder.Services.AddScoped<PasswordValidator>();
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

/* THIS SECTION SEEDS THE DATABASE 
  using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);  // THIS LINE MUST BE HERE!
}
*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//These replace MapStaticAssets and WithStaticAssets
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
