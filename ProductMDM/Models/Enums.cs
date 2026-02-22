namespace ProductMDM.Models
{
    /// <summary>
    /// Product lifecycle status in MDM.
    /// </summary>
    public enum ProductStatus
    {
        Draft = 0,
        Active = 1,
        Discontinued = 2
    }

    /// <summary>
    /// Logical data types for product attributes.
    /// Stored as string in v1 but type is recorded for consumers.
    /// </summary>
    public enum AttributeDataType
    {
        String = 0,
        Int = 1,
        Decimal = 2,
        Bool = 3,
        Date = 4
    }
}
