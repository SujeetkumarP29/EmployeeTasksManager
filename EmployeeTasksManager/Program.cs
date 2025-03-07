using EmployeeTasksManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add EF Core and configure SQL Server database
builder.Services.AddDbContext<EmployeeTasksManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Configure Identity (Remove separate Cookie Authentication)
builder.Services.AddIdentity<Employee, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // No email confirmation required
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<EmployeeTasksManagerContext>()
    .AddDefaultTokenProviders();

// 🔹 Configure Identity Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";  // Redirect if not authenticated
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);  // Session expiration
    options.SlidingExpiration = true;  // Extends session on user activity
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 64; // Increase depth if needed
});

builder.Services.AddAuthorization();

// 🔹 Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Middleware setup
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
