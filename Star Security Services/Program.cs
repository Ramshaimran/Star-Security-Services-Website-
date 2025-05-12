using Microsoft.EntityFrameworkCore;
using Star_Security_Services.Models;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);  // Set the session timeout duration
    options.Cookie.HttpOnly = true;                  // Restrict client-side access to the session cookie
    options.Cookie.IsEssential = true;               // Make the session cookie essential for the app
});

// Add controllers and views
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<mycontext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Defaults"));
});

// Add Razor Pages (needed for some MVC scenarios)
builder.Services.AddRazorPages();

var app = builder.Build();

// Middleware pipeline

// Use HTTPS redirection
app.UseHttpsRedirection();

// Enable static file serving (important for serving images like QR codes, CSS, JS, etc.)
app.UseStaticFiles();  // This ensures static files in wwwroot are served

// Enable Session management middleware - must be called before any routing
app.UseSession();

// Routing setup
app.UseRouting();

// Authorization middleware - use this if you have authentication/authorization
app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

// Map default route and allow for controllers and Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=home}/{id?}");
app.MapRazorPages(); // Ensure Razor Pages are mapped

// Start the app
app.Run();
