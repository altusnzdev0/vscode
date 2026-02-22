using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IndexModel(ApplicationDbContext db) { _db = db; }
        public List<dynamic> Items { get; set; } = new();
        public async Task OnGetAsync()
        {
            Items = await _db.Categories.Include(c => c.Parent).OrderBy(c => c.Name).Select(c => new { c.CategoryId, c.Name, ParentName = c.Parent != null ? c.Parent.Name : string.Empty }).ToListAsync<dynamic>();
        }
    }
}
