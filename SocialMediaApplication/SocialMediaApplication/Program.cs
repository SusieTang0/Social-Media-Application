using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMediaApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add data protection services with persistent key storage
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
    .SetApplicationName("SocialMediaApplication") // Use the same application name across all instances
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Adjust key lifetime as needed

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register FirebaseService as a singleton
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSingleton<FirebaseService2>();
builder.Services.AddScoped<PostService>();

// Add session support (if needed)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session middleware
app.UseAuthentication(); // If using authentication middleware
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
