using Microsoft.EntityFrameworkCore;
using ProductMDM.Models;

namespace ProductMDM.Data
{
    /// <summary>
    /// EF Core DbContext for ProductMDM. Configures entities and constraints using Fluent API.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
        public DbSet<PriceList> PriceLists => Set<PriceList>();
        public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
        public DbSet<ProductRelation> ProductRelations => Set<ProductRelation>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Brands
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(b => b.BrandId);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(200).HasComment("Brand display name");
                entity.Property(b => b.Description).HasMaxLength(1000).HasComment("Optional brand description");
                entity.HasIndex(b => b.Name).IsUnique();
                entity.ToTable(tb => tb.HasComment("Brands that products belong to"));
            });

            // Categories (self referencing)
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200).HasComment("Category name");
                entity.HasOne(c => c.Parent)
                      .WithMany(c => c.Children)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable(tb => tb.HasComment("Hierarchical product categories"));
            });

            // Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.SKU).IsRequired().HasMaxLength(100).HasComment("Stock Keeping Unit");
                entity.Property(p => p.Name).IsRequired().HasMaxLength(400).HasComment("Product display name");
                entity.Property(p => p.Description).HasMaxLength(4000).HasComment("Product description for catalogue/admin");
                entity.HasIndex(p => p.SKU).IsUnique();
                entity.HasOne(p => p.Brand).WithMany().HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Category).WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.CreatedAt).HasComment("Record creation timestamp");
                entity.Property(p => p.UpdatedAt).HasComment("Record last update timestamp");
                entity.ToTable(tb => tb.HasComment("Master product record storing core fields and publication status"));
            });

            // ProductAttribute
            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.HasKey(a => a.AttributeId);
                entity.Property(a => a.Key).IsRequired().HasMaxLength(200).HasComment("Attribute key/name");
                entity.Property(a => a.Value).HasMaxLength(2000).HasComment("Attribute value stored as string");
                entity.HasOne(a => a.Product).WithMany(p => p.Attributes).HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(a => new { a.ProductId, a.Key }).IsUnique();
                entity.Property(a => a.DataType).HasComment("Logical datatype of the attribute");
                entity.ToTable(tb => tb.HasComment("Free-form attributes for products. Keys unique per product"));
            });

            // PriceList
            modelBuilder.Entity<PriceList>(entity =>
            {
                entity.HasKey(pl => pl.PriceListId);
                entity.Property(pl => pl.Name).IsRequired().HasMaxLength(200);
                entity.Property(pl => pl.Currency).IsRequired().HasMaxLength(3).HasComment("ISO 4217 currency code");
                entity.HasIndex(pl => pl.IsDefault);
                entity.ToTable(tb => tb.HasComment("Price lists used for different pricing scenarios"));
            });

            // ProductPrice
            modelBuilder.Entity<ProductPrice>(entity =>
            {
                entity.HasKey(pp => pp.ProductPriceId);
                entity.Property(pp => pp.ListPrice).HasColumnType("decimal(18,4)");
                entity.HasOne(pp => pp.Product).WithMany(p => p.Prices).HasForeignKey(pp => pp.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(pp => pp.PriceList).WithMany().HasForeignKey(pp => pp.PriceListId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(pp => new { pp.ProductId, pp.PriceListId, pp.EffectiveFrom }).IsUnique();
                entity.ToTable(tb => tb.HasComment("Prices for products per price list and effective date"));
            });

            // ProductRelation
            modelBuilder.Entity<ProductRelation>(entity =>
            {
                entity.HasKey(pr => pr.ProductRelationId);
                entity.HasOne(pr => pr.Product).WithMany(p => p.Relations).HasForeignKey(pr => pr.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne<Product>().WithMany().HasForeignKey(pr => pr.RelatedProductId).OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(pr => new { pr.ProductId, pr.RelatedProductId, pr.RelationType }).IsUnique();
                entity.ToTable(tb => tb.HasComment("Relations between products (accessories, alternatives, etc.)"));
            });

            // ProductImage
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(pi => pi.ProductImageId);
                entity.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(2000);
                entity.Property(pi => pi.Caption).HasMaxLength(500);
                entity.HasOne(pi => pi.Product).WithMany(p => p.Images).HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.ToTable(tb => tb.HasComment("Image references for products; v1 stores URLs only"));
            });
        }
    }
}
