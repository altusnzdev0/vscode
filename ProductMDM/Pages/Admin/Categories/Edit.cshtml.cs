using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditModel(ApplicationDbContext db) { _db = db; }

        [BindProperty]
        public Category Category { get; set; } = new();

        public bool IsNew => Category.CategoryId == 0;
        public List<SelectListItem> ParentOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ParentOptions = await _db.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem(c.Name, c.CategoryId.ToString())).ToListAsync();

            if (id.HasValue)
            {
                var c = await _db.Categories.FindAsync(id.Value);
                if (c == null) return NotFound();
                Category = c;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            Category.Name = Category.Name?.Trim() ?? string.Empty;

            if (Category.ParentCategoryId == Category.CategoryId) ModelState.AddModelError("Category.ParentCategoryId", "Parent category cannot be itself.");
            if (!ModelState.IsValid) return Page();

            if (Category.CategoryId == 0) _db.Categories.Add(Category);
            else _db.Categories.Update(Category);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Category saved.";
            return RedirectToPage("/Admin/Categories/Index");
        }
    }
}
