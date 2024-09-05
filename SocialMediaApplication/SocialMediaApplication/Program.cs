using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMediaApplication.Services.FollowService;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add data protection services with persistent key storage
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
    .SetApplicationName("SocialMediaApplication") // Use the same application name across all instances
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Adjust key lifetime as needed

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register UserService
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

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

app.Use(async (context, next) =>
{
    if (string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) &&
        !context.Request.Path.StartsWithSegments("/api"))
    {
        await context.RequestServices.GetRequiredService<IAntiforgery>()
            .ValidateRequestAsync(context);
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Follow}/{action=Follow}/{id?}");

app.Run();
