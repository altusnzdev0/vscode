using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Pages.Admin.Products
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public string? Search { get; set; }
        public int? BrandId { get; set; }

        public List<dynamic> Items { get; set; } = new();
        public List<SelectListItem> BrandOptions { get; set; } = new();

        public async Task OnGetAsync(string? search, int? brandId)
        {
            Search = search;
            BrandId = brandId;

            BrandOptions = await _db.Brands.OrderBy(b => b.Name).Select(b => new SelectListItem(b.Name, b.BrandId.ToString())).ToListAsync();

            var defaultPriceListId = await _db.PriceLists.Where(pl => pl.IsDefault).Select(pl => pl.PriceListId).FirstOrDefaultAsync();

            var q = _db.Products.Include(p => p.Brand).Include(p => p.Prices).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            if (brandId.HasValue) q = q.Where(p => p.BrandId == brandId.Value);

            Items = await q.OrderBy(p => p.Name).Take(50).Select(p => new
            {
                p.ProductId,
                p.SKU,
                p.Name,
                BrandName = p.Brand != null ? p.Brand.Name : "",
                DefaultPrice = p.Prices!.Where(pp => pp.PriceListId == defaultPriceListId).OrderByDescending(pp => pp.EffectiveFrom).Select(pp => pp.ListPrice).FirstOrDefault()
            }).ToListAsync<dynamic>();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (id == 0) return BadRequest();

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToPage();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Product deleted.";
            return RedirectToPage();
        }
    }
}
