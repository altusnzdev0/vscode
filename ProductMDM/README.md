# ProductMDM (v1) - Product Master Data

Brief: Internal Product Master Data (MDM) app v1. Razor Pages admin UI, read-only catalogue, and read-only APIs for external consumers.

Getting started
- Database name: ProductMDM
- To create schema and sample data run:

```sql
-- run on your SQL Server instance
:r scripts/create_database.sql
GO
:r scripts/sample_data.sql
GO
```

- Update `appsettings.json` `ConnectionStrings:DefaultConnection` to point to your SQL Server.
- Build & run:

```bash
dotnet build
dotnet run --project ProductMDM.csproj
```

Developer shortcut (no SQL Server required)
- For quick local testing without SQL Server, enable a SQLite dev fallback. This creates a small `dev_productmdm.db` file in the app folder and ensures the EF schema exists.

Windows PowerShell:

```powershell
$env:DEV_FALLBACK_SQLITE = 'true'
dotnet run --project ProductMDM.csproj
```

Notes
- To use the real SQL Server database, run `scripts/create_database.sql` and `scripts/sample_data.sql` on your SQL Server instance, then update `appsettings.json` with the connection string.
- Swagger UI is enabled when `ASPNETCORE_ENVIRONMENT` is `Development` (see `Properties/launchSettings.json`).

Local URLs
- Swagger: http://localhost:5000/swagger
- Catalogue: http://localhost:5000/Catalogue
- Admin: http://localhost:5000/Admin

APIs
- GET /api/products?search=&brandId=&categoryId=&page=&pageSize=&minPrice=&maxPrice=&isPublished=
- GET /api/products/{id}
- GET /api/products/{id}/attributes
- GET /api/products/{id}/prices
- GET /api/products/{id}/related
- GET /api/brands
- GET /api/categories
- GET /api/metadata

Notes & Assumptions
- v1 intentionally excludes authentication; plan for Entra ID integration documented for v2.
- Catalog and Admin are internal-only; CSP is permissive for embedding during dev. Tighten frame-ancestors in v2 when auth is added.
- SQL scripts include extended properties on tables/columns describing purpose.

Solution layout
- `ProductMDM.csproj` - project file
- `Program.cs` - app startup
- `Data/ApplicationDbContext.cs` - EF Core context and model configuration
- `Models/` - domain entities and enums
- `Controllers/Api` - read-only API controllers
- `Pages/Admin` - Razor Pages for admin MDM tasks
- `Pages/Catalogue` - read-only catalogue pages
- `scripts/` - SQL scripts (create_database.sql, sample_data.sql)
- `ERD.md` - mermaid ERD

If anything is unclear, open an issue or reply here and I will refine.
