using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IProductService"/> using EF Core.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetDefaultPriceListIdAsync(CancellationToken cancellationToken = default)
        {
            return await _db.PriceLists.Where(pl => pl.IsDefault).Select(pl => pl.PriceListId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Product?> GetProductWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Products
                .Include(p => p.Attributes)
                .Include(p => p.Prices)
                .Include(p => p.Images)
                .Include(p => p.Relations)
                .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
        }
    }
}
