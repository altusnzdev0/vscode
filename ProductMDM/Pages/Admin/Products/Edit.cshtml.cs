using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;
using ProductMDM.Models;

namespace ProductMDM.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        public bool IsNew => Product.ProductId == 0;
        public List<SelectListItem> BrandOptions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            BrandOptions = await _db.Brands.OrderBy(b => b.Name).Select(b => new SelectListItem(b.Name, b.BrandId.ToString())).ToListAsync();
            CategoryOptions = await _db.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem(c.Name, c.CategoryId.ToString())).ToListAsync();

            if (id.HasValue)
            {
                var product = await _db.Products
                    .Include(p => p.Attributes)
                    .Include(p => p.Prices)
                    .Include(p => p.Images)
                    .Include(p => p.Relations)
                    .FirstOrDefaultAsync(p => p.ProductId == id.Value);
                if (product == null) return NotFound();
                Product = product;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Server-side trimming & validation
            Product.SKU = Product.SKU?.Trim() ?? string.Empty;
            Product.Name = Product.Name?.Trim() ?? string.Empty;

            if (Product.ProductId == 0)
            {
                Product.CreatedAt = DateTime.UtcNow;
                Product.UpdatedAt = DateTime.UtcNow;
                _db.Products.Add(Product);
            }
            else
            {
                Product.UpdatedAt = DateTime.UtcNow;
                _db.Products.Update(Product);
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Product saved.";
            return RedirectToPage("/Admin/Products/Index");
        }

        public async Task<IActionResult> OnPostAddAttributeAsync(string Key, string? Value, int DataType, int SortOrder = 100)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                ModelState.AddModelError("Key", "Key is required");
                return Page();
            }

            // Ensure product exists
            var product = await _db.Products.Include(p => p.Attributes).FirstOrDefaultAsync(p => p.ProductId == Product.ProductId);
            if (product == null) return NotFound();

            var trimmedKey = Key.Trim();
            var existing = product.Attributes?.FirstOrDefault(a => a.Key == trimmedKey);
            if (existing != null)
            {
                existing.Value = Value;
                existing.DataType = (ProductMDM.Models.AttributeDataType)DataType;
                existing.SortOrder = SortOrder;
                _db.ProductAttributes.Update(existing);
            }
            else
            {
                var attr = new ProductMDM.Models.ProductAttribute
                {
                    ProductId = product.ProductId,
                    Key = trimmedKey,
                    Value = Value,
                    DataType = (ProductMDM.Models.AttributeDataType)DataType,
                    SortOrder = SortOrder
                };
                _db.ProductAttributes.Add(attr);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Attribute added/updated.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAttributeAsync(int attributeId)
        {
            var a = await _db.ProductAttributes.FindAsync(attributeId);
            if (a != null)
            {
                _db.ProductAttributes.Remove(a);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Attribute deleted.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddRelationAsync(int ProductId, int RelatedProductId, string? RelationType)
        {
            if (ProductId == 0 || RelatedProductId == 0) return BadRequest();
            var exists = await _db.ProductRelations.FirstOrDefaultAsync(r => r.ProductId == ProductId && r.RelatedProductId == RelatedProductId && r.RelationType == RelationType);
            if (exists == null)
            {
                _db.ProductRelations.Add(new ProductMDM.Models.ProductRelation { ProductId = ProductId, RelatedProductId = RelatedProductId, RelationType = RelationType });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Relation added.";
            }
            else
            {
                TempData["Success"] = "Relation already exists.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteRelationAsync(int relationId)
        {
            var r = await _db.ProductRelations.FindAsync(relationId);
            if (r != null)
            {
                _db.ProductRelations.Remove(r);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Relation removed.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddImageAsync(int ProductId, string ImageUrl, string? Caption, int SortOrder = 0)
        {
            if (ProductId == 0 || string.IsNullOrWhiteSpace(ImageUrl)) return BadRequest();
            var img = new ProductMDM.Models.ProductImage { ProductId = ProductId, ImageUrl = ImageUrl.Trim(), Caption = Caption, SortOrder = SortOrder, IsPrimary = false };
            _db.ProductImages.Add(img);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Image added.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSetPrimaryImageAsync(int imageId)
        {
            var img = await _db.ProductImages.FindAsync(imageId);
            if (img == null) return NotFound();
            // Clear existing primary
            var others = await _db.ProductImages.Where(i => i.ProductId == img.ProductId && i.IsPrimary).ToListAsync();
            foreach (var o in others) { o.IsPrimary = false; }
            img.IsPrimary = true;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Primary image set.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(int imageId)
        {
            var img = await _db.ProductImages.FindAsync(imageId);
            if (img != null)
            {
                _db.ProductImages.Remove(img);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Image deleted.";
            }
            return RedirectToPage();
        }
    }
}
