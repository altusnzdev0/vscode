using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Relation between products (e.g., accessory, similar)
    /// Unique per (ProductId, RelatedProductId, RelationType)
    /// </summary>
    public class ProductRelation
    {
        public int ProductRelationId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int RelatedProductId { get; set; }

        [StringLength(200)]
        public string? RelationType { get; set; }
    }
}
