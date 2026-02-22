USE [ProductMDM];
GO

-- Insert Brands with fixed IDs
SET IDENTITY_INSERT dbo.Brands ON;
INSERT INTO dbo.Brands (BrandId, Name, Description) VALUES (1, N'Altus', N'Altus brand');
INSERT INTO dbo.Brands (BrandId, Name, Description) VALUES (2, N'Atlantic', N'Atlantic brand');
SET IDENTITY_INSERT dbo.Brands OFF;
GO

-- Insert Categories, ensure CategoryId 16 exists for sample product
SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (CategoryId, Name, ParentCategoryId) VALUES (1, N'Bifold Hardware', NULL);
INSERT INTO dbo.Categories (CategoryId, Name, ParentCategoryId) VALUES (2, N'Sliding Door Trim/Track', NULL);
INSERT INTO dbo.Categories (CategoryId, Name, ParentCategoryId) VALUES (16, N'Some Category 16', 2);
INSERT INTO dbo.Categories (CategoryId, Name, ParentCategoryId) VALUES (17, N'Some Category 17', 2);
SET IDENTITY_INSERT dbo.Categories OFF;
GO

-- Price lists
SET IDENTITY_INSERT dbo.PriceLists ON;
INSERT INTO dbo.PriceLists (PriceListId, [Name], Currency, IsDefault) VALUES (1, N'Default', N'NZD', 1);
INSERT INTO dbo.PriceLists (PriceListId, [Name], Currency, IsDefault) VALUES (2, N'Stocked Colour', N'NZD', 0);
SET IDENTITY_INSERT dbo.PriceLists OFF;
GO

-- Insert sample product with ProductId = 8
SET IDENTITY_INSERT dbo.Products ON;
INSERT INTO dbo.Products (ProductId, SKU, Name, Description, BrandId, CategoryId, Status, IsPublished, CreatedAt, UpdatedAt)
VALUES (8, N'72370B', N'MB72370B Atlantic Bifold 2P Top Roller', N'MB72370B Atlantic Bifold 2P Top Roller', 1, 16, 1, 1, '2026-02-12T20:15:38', '2026-02-12T20:15:38');
SET IDENTITY_INSERT dbo.Products OFF;
GO

-- Product Attributes for product 8
SET IDENTITY_INSERT dbo.ProductAttributes ON;
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (1,8,N'Product Code',N'E14916',0,10);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (2,8,N'SAP No',N'87654',0,20);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (3,8,N'Product Description',N'Arch-Millennium -N',0,30);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (4,8,N'Product Family',N'Extrusion',0,40);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (5,8,N'Product Hierarchy',N'900012117531200178',0,50);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (6,8,N'Product Name',N'S41 ES TRK',0,60);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (7,8,N'Product SKU',N'E14916',0,70);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (8,8,N'AM2perMeter',N'0.1000',2,80);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (9,8,N'Basic Material Description',N'S41 ES TRK',0,90);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (10,8,N'Broken Die',N'Yes',0,100);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (11,8,N'Catalog Code',N'TRT',0,110);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (12,8,N'Colour Group',N'AS',0,120);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (13,8,N'Die - Section',N'E14916',0,130);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (14,8,N'Die Due Back In Service',N'2026-01-01',4,140);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (15,8,N'External ID',N'T67Y5998T',0,150);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (16,8,N'Has Length',N'True',3,160);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (17,8,N'Has Stock Outs',N'False',3,170);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (18,8,N'Hierarchy Description',N'TB Sliding Door Trim/Track',0,180);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (19,8,N'Image Path',N'https://altuscommunity--uat.sandbox.my.site.com/sfsites/c/cms/delivery/media/MCBP3BNM6TWVFSXEJQ33KRAGHW6Q&cb=05TIo000000GX4e',0,190);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (20,8,N'Item Code',N'E24916',0,200);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (21,8,N'Item Type',N'Extrusion',0,210);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (22,8,N'KGPerMeter',N'0.2060',2,220);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (23,8,N'Leadtime',N'2',1,230);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (24,8,N'Linked Hierarchy Group',N'98',1,240);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (25,8,N'Mat Price Grp',N'ZE',0,250);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (26,8,N'Max Length',N'6.0000',2,260);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (27,8,N'Not Discountable',N'True',3,270);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (28,8,N'Pack Price',N'$15.0000',0,280);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (29,8,N'Pack Size',N'500',1,290);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (30,8,N'ProductCodeItemCodeMatch',N'True',3,300);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (31,8,N'Quantity Unit Of Measure',N'EA',0,310);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (32,8,N'Stocked Hardware',N'True',3,320);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (33,8,N'Stocked Out Lengths Count',N'5',1,330);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (34,8,N'TB',N'NONE',0,340);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (35,8,N'TB Surcharge',N'1.9800',2,350);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (36,8,N'Total List Price',N'56.0000',2,360);
INSERT INTO dbo.ProductAttributes (AttributeId, ProductId, [Key], [Value], DataType, SortOrder) VALUES (37,8,N'Weight',N'55',1,370);
SET IDENTITY_INSERT dbo.ProductAttributes OFF;
GO

-- Product Price for product 8 in default price list
INSERT INTO dbo.ProductPrices (ProductId, PriceListId, ListPrice, EffectiveFrom) VALUES (8, 1, 56.0000, '2026-02-12T00:00:00');
GO

-- Product relation: 8 -> 9 as Accessory (ensure product 9 exists? Insert a minimal product 9)
SET IDENTITY_INSERT dbo.Products ON;
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductId = 9)
    INSERT INTO dbo.Products (ProductId, SKU, Name, Description, BrandId, CategoryId, Status, IsPublished, CreatedAt, UpdatedAt)
    VALUES (9, N'EXAMPLE9', N'Accessory Example', N'Accessory product example', 1, 16, 1, 1, GETUTCDATE(), GETUTCDATE());
SET IDENTITY_INSERT dbo.Products OFF;
GO

INSERT INTO dbo.ProductRelations (ProductId, RelatedProductId, RelationType) VALUES (8,9,N'Accessory');
GO

-- Product image for product 8
INSERT INTO dbo.ProductImages (ProductId, ImageUrl, Caption, SortOrder, IsPrimary) VALUES (8, N'https://altuscommunity--uat.sandbox.my.site.com/sfsites/c/cms/delivery/media/MCBP3BNM6TWVFSXEJQ33KRAGHW6Q&cb=05TIo000000GX4e', N'Primary image', 1, 1);
GO
