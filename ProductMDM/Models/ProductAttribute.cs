using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Free-form attribute for a product. Keys must be unique per product.
    /// Value stored as string; DataType describes how to interpret it.
    /// </summary>
    public class ProductAttribute
    {
        public int AttributeId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(200)]
        public string Key { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Value { get; set; }

        public AttributeDataType DataType { get; set; }

        public int SortOrder { get; set; }
    }
}
