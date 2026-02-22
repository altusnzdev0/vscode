using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Pages.Catalogue
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public DetailsModel(ApplicationDbContext db) { _db = db; }

        public ProductMDM.Models.Product? Product { get; set; }
        public string? PrimaryImage { get; set; }
        public decimal? DefaultPrice { get; set; }
        public List<ProductMDM.Models.ProductAttribute> Attributes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _db.Products.Include(p => p.Images).Include(p => p.Attributes).Include(p => p.Prices).FirstOrDefaultAsync(p => p.ProductId == id);
            if (Product == null) return NotFound();
            PrimaryImage = Product.Images?.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() ?? Product.Images?.Select(i => i.ImageUrl).FirstOrDefault();
            var defaultPriceListId = await _db.PriceLists.Where(pl => pl.IsDefault).Select(pl => pl.PriceListId).FirstOrDefaultAsync();
            DefaultPrice = Product.Prices?.Where(pp => pp.PriceListId == defaultPriceListId).OrderByDescending(pp => pp.EffectiveFrom).Select(pp => pp.ListPrice).FirstOrDefault();
            Attributes = Product.Attributes?.OrderBy(a => a.SortOrder).ToList() ?? new List<ProductMDM.Models.ProductAttribute>();
            return Page();
        }
    }
}
