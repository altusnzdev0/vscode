using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Pages.Admin.Brands
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) { _db = db; }
        public List<dynamic> Items { get; set; } = new();
        public async Task OnGetAsync()
        {
            Items = await _db.Brands.OrderBy(b => b.Name).Select(b => new { b.BrandId, b.Name }).ToListAsync<dynamic>();
        }
    }
}
