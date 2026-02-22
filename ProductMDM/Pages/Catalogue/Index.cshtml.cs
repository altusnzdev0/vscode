using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Pages.Catalogue
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public string? Search { get; set; }

        public List<dynamic> Items { get; set; } = new();

        public async Task OnGetAsync(string? search)
        {
            Search = search;
            var defaultPriceListId = await _db.PriceLists.Where(pl => pl.IsDefault).Select(pl => pl.PriceListId).FirstOrDefaultAsync();

            var q = _db.Products.Include(p => p.Images).Include(p => p.Prices).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));

            Items = await q.OrderBy(p => p.Name).Take(24).Select(p => new
            {
                p.ProductId,
                p.SKU,
                p.Name,
                p.Description,
                PrimaryImage = p.Images!.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() ?? p.Images!.Select(i => i.ImageUrl).FirstOrDefault(),
                DefaultPrice = p.Prices!.Where(pp => pp.PriceListId == defaultPriceListId).OrderByDescending(pp => pp.EffectiveFrom).Select(pp => pp.ListPrice).FirstOrDefault()
            }).ToListAsync<dynamic>();
        }
    }
}
