using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductMDM.Models
{
    /// <summary>
    /// Master product record.
    /// </summary>
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [StringLength(400)]
        public string Name { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Description { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ProductStatus Status { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ProductAttribute>? Attributes { get; set; }
        public ICollection<ProductPrice>? Prices { get; set; }
        public ICollection<ProductRelation>? Relations { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
    }
}
