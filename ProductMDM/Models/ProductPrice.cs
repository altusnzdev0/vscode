using System.ComponentModel.DataAnnotations;

namespace ProductMDM.Models
{
    /// <summary>
    /// Price for a product in a specific PriceList with an effective date.
    /// Unique per (ProductId, PriceListId, EffectiveFrom).
    /// </summary>
    public class ProductPrice
    {
        public int ProductPriceId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int PriceListId { get; set; }
        public PriceList? PriceList { get; set; }

        public decimal ListPrice { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
