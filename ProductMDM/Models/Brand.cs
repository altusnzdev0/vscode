using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Brand entity: groups products by manufacturer/brand.
    /// </summary>
    public class Brand
    {
        public int BrandId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
