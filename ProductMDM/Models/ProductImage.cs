using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Image metadata for a product. v1 stores only image URLs.
    /// </summary>
    public class ProductImage
    {
        public int ProductImageId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(2000)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Caption { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }
    }
}
