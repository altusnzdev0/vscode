using ProductMDM.Models;

namespace ProductMDM.Services
{
    /// <summary>
    /// Lightweight service abstraction for common product queries used by pages and APIs.
    /// Keeps controller and page code small and testable.
    /// </summary>
    public interface IProductService
    {
        Task<int> GetDefaultPriceListIdAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetProductWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    }
}
