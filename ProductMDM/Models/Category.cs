using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductMDM.Models
{
    /// <summary>
    /// Category in a self-referencing hierarchy.
    /// </summary>
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public Category? Parent { get; set; }

        public ICollection<Category>? Children { get; set; }
    }
}
