using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Price list defining a currency and usage (default price list indicated with IsDefault).
    /// </summary>
    public class PriceList
    {
        public int PriceListId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "NZD";

        public bool IsDefault { get; set; }
    }
}
