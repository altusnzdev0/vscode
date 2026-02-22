-- Create database and schema for ProductMDM
-- Run on SQL Server as a user that can create databases.

IF DB_ID(N'ProductMDM') IS NULL
BEGIN
    CREATE DATABASE [ProductMDM];
END
GO

USE [ProductMDM];
GO

-- Brands
IF OBJECT_ID('dbo.Brands') IS NULL
BEGIN
CREATE TABLE dbo.Brands
(
    BrandId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL
);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Brands that products belong to', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = Brands;
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Brand display name', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = Brands, @level2type = N'Column', @level2name = Name;
END
GO

-- Categories
IF OBJECT_ID('dbo.Categories') IS NULL
BEGIN
CREATE TABLE dbo.Categories
(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    ParentCategoryId INT NULL
);
ALTER TABLE dbo.Categories ADD CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES dbo.Categories(CategoryId) ON DELETE NO ACTION;
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Hierarchical product categories', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = Categories;
END
GO

-- Products
IF OBJECT_ID('dbo.Products') IS NULL
BEGIN
CREATE TABLE dbo.Products
(
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    SKU NVARCHAR(100) NOT NULL,
    Name NVARCHAR(400) NOT NULL,
    Description NVARCHAR(4000) NULL,
    BrandId INT NOT NULL,
    CategoryId INT NOT NULL,
    Status INT NOT NULL,
    IsPublished BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);
ALTER TABLE dbo.Products ADD CONSTRAINT FK_Products_Brand FOREIGN KEY (BrandId) REFERENCES dbo.Brands(BrandId) ON DELETE NO ACTION;
ALTER TABLE dbo.Products ADD CONSTRAINT FK_Products_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId) ON DELETE NO ACTION;
CREATE UNIQUE INDEX UX_Products_SKU ON dbo.Products(SKU);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Master product record storing core fields and publication status', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = Products;
END
GO

-- ProductAttributes
IF OBJECT_ID('dbo.ProductAttributes') IS NULL
BEGIN
CREATE TABLE dbo.ProductAttributes
(
    AttributeId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    [Key] NVARCHAR(200) NOT NULL,
    [Value] NVARCHAR(2000) NULL,
    DataType INT NOT NULL DEFAULT 0,
    SortOrder INT NOT NULL DEFAULT 0
);
ALTER TABLE dbo.ProductAttributes ADD CONSTRAINT FK_ProductAttributes_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId) ON DELETE CASCADE;
CREATE UNIQUE INDEX UX_ProductAttributes_ProductKey ON dbo.ProductAttributes(ProductId, [Key]);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Free-form attributes for products. Keys unique per product', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = ProductAttributes;
END
GO

-- PriceLists
IF OBJECT_ID('dbo.PriceLists') IS NULL
BEGIN
CREATE TABLE dbo.PriceLists
(
    PriceListId INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    Currency NVARCHAR(3) NOT NULL,
    IsDefault BIT NOT NULL DEFAULT 0
);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Price lists used for different pricing scenarios', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = PriceLists;
END
GO

-- ProductPrices
IF OBJECT_ID('dbo.ProductPrices') IS NULL
BEGIN
CREATE TABLE dbo.ProductPrices
(
    ProductPriceId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    PriceListId INT NOT NULL,
    ListPrice DECIMAL(18,4) NOT NULL,
    EffectiveFrom DATETIME2 NOT NULL,
    EffectiveTo DATETIME2 NULL
);
ALTER TABLE dbo.ProductPrices ADD CONSTRAINT FK_ProductPrices_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId) ON DELETE CASCADE;
ALTER TABLE dbo.ProductPrices ADD CONSTRAINT FK_ProductPrices_PriceList FOREIGN KEY (PriceListId) REFERENCES dbo.PriceLists(PriceListId) ON DELETE NO ACTION;
CREATE UNIQUE INDEX UX_ProductPrices_ProductPriceListEffective ON dbo.ProductPrices(ProductId, PriceListId, EffectiveFrom);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Prices for products per price list and effective date', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = ProductPrices;
END
GO

-- ProductRelations
IF OBJECT_ID('dbo.ProductRelations') IS NULL
BEGIN
CREATE TABLE dbo.ProductRelations
(
    ProductRelationId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    RelatedProductId INT NOT NULL,
    RelationType NVARCHAR(200) NULL
);
ALTER TABLE dbo.ProductRelations ADD CONSTRAINT FK_ProductRelations_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId) ON DELETE CASCADE;
ALTER TABLE dbo.ProductRelations ADD CONSTRAINT FK_ProductRelations_RelatedProduct FOREIGN KEY (RelatedProductId) REFERENCES dbo.Products(ProductId) ON DELETE NO ACTION;
CREATE UNIQUE INDEX UX_ProductRelations_ProductRelatedType ON dbo.ProductRelations(ProductId, RelatedProductId, RelationType);
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Relations between products (accessory, similar, etc.)', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = ProductRelations;
END
GO

-- ProductImages
IF OBJECT_ID('dbo.ProductImages') IS NULL
BEGIN
CREATE TABLE dbo.ProductImages
(
    ProductImageId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    ImageUrl NVARCHAR(2000) NOT NULL,
    Caption NVARCHAR(500) NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsPrimary BIT NOT NULL DEFAULT 0
);
ALTER TABLE dbo.ProductImages ADD CONSTRAINT FK_ProductImages_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId) ON DELETE CASCADE;
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Image references for products; stores URLs in v1', @level0type = N'Schema', @level0name = dbo, @level1type = N'Table', @level1name = ProductImages;
END
GO

-- Indexes for performance (SQL Server compatible)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_BrandId' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    CREATE INDEX IX_Products_BrandId ON dbo.Products(BrandId);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_CategoryId' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
END

GO
