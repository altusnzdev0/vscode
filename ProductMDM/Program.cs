using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure logging level via appsettings. Use "DetailedLogging": true for debug.
builder.Logging.AddConsole();

// Configuration: connection string should point to your SQL Server instance.
// For convenient local testing when SQL Server is not available, set environment
// variable DEV_FALLBACK_SQLITE=true to use a lightweight SQLite DB file.
var useFallbackSqlite = builder.Configuration.GetValue<bool>("DEV_FALLBACK_SQLITE") || Environment.GetEnvironmentVariable("DEV_FALLBACK_SQLITE") == "true";
if (useFallbackSqlite)
{
    var sqliteFile = Path.Combine(AppContext.BaseDirectory, "dev_productmdm.db");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite($"Data Source={sqliteFile}"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
                             "Server=(localdb)\\mssqllocaldb;Database=ProductMDM;Trusted_Connection=True;MultipleActiveResultSets=true"));
}

// Add services for Razor Pages and MVC controllers (for APIs)
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Application services
builder.Services.AddScoped<ProductMDM.Services.IProductService, ProductMDM.Services.ProductService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// If using fallback sqlite in development, ensure database is created so pages can render.
if (useFallbackSqlite)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Enable static files from wwwroot
app.UseStaticFiles();

// Simple permissive CSP/frame-ancestors for dev embedding in SharePoint/Teams.
// NOTE: Tighten in v2 when Entra ID is added. See README for guidance.
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "frame-ancestors *;";
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.RoutePrefix = "swagger");
}
else
{
    app.UseExceptionHandler("/Error");
}

app.MapControllers();
app.MapRazorPages();

app.Run();
