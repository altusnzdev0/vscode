using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Pages.Admin.Brands
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Brand Brand { get; set; } = new();

        public bool IsNew => Brand.BrandId == 0;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var b = await _db.Brands.FindAsync(id.Value);
                if (b == null) return NotFound();
                Brand = b;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Brand.Name = Brand.Name?.Trim() ?? string.Empty;

            if (Brand.BrandId == 0) _db.Brands.Add(Brand);
            else _db.Brands.Update(Brand);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Brand saved.";
            return RedirectToPage("/Admin/Brands/Index");
        }
    }
}
