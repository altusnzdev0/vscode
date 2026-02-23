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

    // If DB is empty, try to seed minimal sample data from api_products.json (vended with the project)
    try
    {
        if (!db.Products.Any())
        {
            var file = Path.Combine(AppContext.BaseDirectory, "api_products.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("items", out var items))
                {
                    var defaultPriceList = db.PriceLists.FirstOrDefault(pl => pl.IsDefault) ?? new ProductMDM.Models.PriceList { Name = "Default", Currency = "NZD", IsDefault = true };
                    if (defaultPriceList.PriceListId == 0) db.PriceLists.Add(defaultPriceList);

                    // Create or reuse a fallback Brand and Category so Product foreign keys are satisfied
                    var defaultBrand = db.Brands.FirstOrDefault(b => b.Name == "Altus") ?? new ProductMDM.Models.Brand { Name = "Altus", Description = "Seeded brand" };
                    if (defaultBrand.BrandId == 0) db.Brands.Add(defaultBrand);
                    var defaultCategory = db.Categories.FirstOrDefault(c => c.Name == "Uncategorized") ?? new ProductMDM.Models.Category { Name = "Uncategorized" };
                    if (defaultCategory.CategoryId == 0) db.Categories.Add(defaultCategory);
                    db.SaveChanges();

                    foreach (var item in items.EnumerateArray())
                    {
                        var product = new ProductMDM.Models.Product
                        {
                            SKU = item.GetProperty("sku").GetString() ?? string.Empty,
                            Name = item.GetProperty("name").GetString() ?? string.Empty,
                            Description = item.TryGetProperty("description", out var d) && d.ValueKind != System.Text.Json.JsonValueKind.Null ? d.GetString() : null,
                            BrandId = defaultBrand.BrandId,
                            CategoryId = defaultCategory.CategoryId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsPublished = true
                        };
                        db.Products.Add(product);
                        db.SaveChanges();

                        if (item.TryGetProperty("primaryImage", out var img) && img.ValueKind != System.Text.Json.JsonValueKind.Null)
                        {
                            db.ProductImages.Add(new ProductMDM.Models.ProductImage { ProductId = product.ProductId, ImageUrl = img.GetString() ?? string.Empty, IsPrimary = true });
                        }

                        if (item.TryGetProperty("defaultPrice", out var dp) && dp.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            var price = dp.GetDecimal();
                            if (price > 0)
                            {
                                db.ProductPrices.Add(new ProductMDM.Models.ProductPrice { ProductId = product.ProductId, PriceListId = defaultPriceList.PriceListId, EffectiveFrom = DateTime.UtcNow, ListPrice = price });
                            }
                        }

                        db.SaveChanges();
                    }
                    // Add an agent-created product for testing if it doesn't already exist
                    if (!db.Products.Any(p => p.SKU == "CREATED_BY_AGENT"))
                    {
                        db.Products.Add(new ProductMDM.Models.Product
                        {
                            SKU = "CREATED_BY_AGENT",
                            Name = "Created By Agent",
                            Description = "Product created programmatically for testing",
                            BrandId = defaultBrand.BrandId,
                            CategoryId = defaultCategory.CategoryId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsPublished = true
                        });
                        db.SaveChanges();
                    }
                }
            }
        }
    }
    catch
    {
        // Swallow any seeding errors in dev so startup isn't blocked. Inspect logs if needed.
    }
}

// Ensure test product exists even if DB already had data
if (useFallbackSqlite)
{
    using var scope2 = app.Services.CreateScope();
    var db2 = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        var brand = db2.Brands.FirstOrDefault(b => b.Name == "Altus");
        var cat = db2.Categories.FirstOrDefault(c => c.Name == "Uncategorized");
        if (brand != null && cat != null && !db2.Products.Any(p => p.SKU == "CREATED_BY_AGENT"))
        {
            db2.Products.Add(new ProductMDM.Models.Product
            {
                SKU = "CREATED_BY_AGENT",
                Name = "Created By Agent",
                Description = "Product created programmatically for testing",
                BrandId = brand.BrandId,
                CategoryId = cat.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPublished = true
            });
            db2.SaveChanges();
        }
    }
    catch { }
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
