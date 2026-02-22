```mermaid
erDiagram
    BRANDS {
        int BrandId PK
        string Name
    }
    CATEGORIES {
        int CategoryId PK
        string Name
        int ParentCategoryId FK
    }
    PRODUCTS {
        int ProductId PK
        string SKU
        string Name
        int BrandId FK
        int CategoryId FK
    }
    PRODUCTATTRIBUTES {
        int AttributeId PK
        int ProductId FK
        string Key
        string Value
    }
    PRICELISTS {
        int PriceListId PK
        string Name
        string Currency
    }
    PRODUCTPRICES {
        int ProductPriceId PK
        int ProductId FK
        int PriceListId FK
        decimal ListPrice
    }
    PRODUCTRELATIONS {
        int ProductRelationId PK
        int ProductId FK
        int RelatedProductId FK
    }
    PRODUCTIMAGES {
        int ProductImageId PK
        int ProductId FK
        string ImageUrl
    }

    BRANDS ||--o{ PRODUCTS : has
    CATEGORIES ||--o{ PRODUCTS : has
    PRODUCTS ||--o{ PRODUCTATTRIBUTES : has
    PRODUCTS ||--o{ PRODUCTPRICES : has
    PRICELISTS ||--o{ PRODUCTPRICES : contains
    PRODUCTS ||--o{ PRODUCTRELATIONS : relates
    PRODUCTS ||--o{ PRODUCTIMAGES : has
```
