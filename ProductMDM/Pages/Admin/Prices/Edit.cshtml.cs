using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Pages.Admin.Prices
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        public Product? Product { get; set; }
        public List<dynamic> Prices { get; set; } = new();
        public List<PriceList> PriceLists { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int productId)
        {
            Product = await _db.Products.FindAsync(productId);
            if (Product == null) return NotFound();

            PriceLists = await _db.PriceLists.OrderBy(pl => pl.Name).ToListAsync();
            Prices = await _db.ProductPrices.Where(pp => pp.ProductId == productId).Include(pp => pp.PriceList).OrderByDescending(pp => pp.EffectiveFrom).Select(pp => new { pp.ProductPriceId, pp.ListPrice, pp.EffectiveFrom, pp.EffectiveTo, PriceListName = pp.PriceList!.Name }).ToListAsync<dynamic>();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int ProductId, int PriceListId, decimal ListPrice, DateTime EffectiveFrom)
        {
            var product = await _db.Products.FindAsync(ProductId);
            if (product == null) return NotFound();

            // Idempotent upsert by (ProductId, PriceListId, EffectiveFrom)
            var existing = await _db.ProductPrices.FirstOrDefaultAsync(pp => pp.ProductId == ProductId && pp.PriceListId == PriceListId && pp.EffectiveFrom == EffectiveFrom);
            if (existing != null)
            {
                existing.ListPrice = ListPrice;
                _db.ProductPrices.Update(existing);
            }
            else
            {
                _db.ProductPrices.Add(new ProductPrice { ProductId = ProductId, PriceListId = PriceListId, ListPrice = ListPrice, EffectiveFrom = EffectiveFrom });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Price upserted.";
            return RedirectToPage(new { productId = ProductId });
        }
    }
}
