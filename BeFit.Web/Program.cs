
using BeFit.Web.Data;
using BeFit.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=BeFitDb.db";
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
}).AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await DataSeeder.SeedAsync(app.Services);

app.Run();
