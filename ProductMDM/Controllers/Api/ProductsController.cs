using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a paged list of products. Read-only API for external consumers.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            string? search,
            int? brandId,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isPublished,
            int page = 1,
            int pageSize = 20)
        {
            var query = _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Prices)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(p => p.Name.Contains(s) || p.SKU.Contains(s));
            }

            if (brandId.HasValue) query = query.Where(p => p.BrandId == brandId.Value);
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
            if (isPublished.HasValue) query = query.Where(p => p.IsPublished == isPublished.Value);

            // Project default price from default PriceList
            var defaultPriceListId = await _db.PriceLists.Where(pl => pl.IsDefault).Select(pl => pl.PriceListId).FirstOrDefaultAsync();

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.ProductId,
                    p.SKU,
                    p.Name,
                    p.Description,
                    Brand = p.Brand != null ? new { p.Brand.BrandId, p.Brand.Name } : null,
                    PrimaryImage = p.Images!.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() ?? p.Images!.Select(i => i.ImageUrl).FirstOrDefault(),
                    DefaultPrice = p.Prices!.Where(pp => pp.PriceListId == defaultPriceListId).OrderByDescending(pp => pp.EffectiveFrom).Select(pp => pp.ListPrice).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new { total, page, pageSize, items });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Attributes)
                .Include(p => p.Prices)
                .Include(p => p.Images)
                .Include(p => p.Relations)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (p == null) return NotFound();

            return Ok(p);
        }

        [HttpGet("{id:int}/attributes")]
        public async Task<IActionResult> GetAttributes(int id)
        {
            var attrs = await _db.ProductAttributes.Where(a => a.ProductId == id).OrderBy(a => a.SortOrder).ToListAsync();
            return Ok(attrs);
        }

        [HttpGet("{id:int}/prices")]
        public async Task<IActionResult> GetPrices(int id)
        {
            var prices = await _db.ProductPrices.Where(pp => pp.ProductId == id).OrderByDescending(pp => pp.EffectiveFrom).ToListAsync();
            return Ok(prices);
        }

        [HttpGet("{id:int}/related")]
        public async Task<IActionResult> GetRelated(int id)
        {
            var rels = await _db.ProductRelations.Where(r => r.ProductId == id).ToListAsync();
            return Ok(rels);
        }
    }
}
