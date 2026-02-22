using Microsoft.AspNetCore.Mvc;
using ProductMDM.Models;
using System.Reflection;

namespace ProductMDM.Controllers.Api
{
    /// <summary>
    /// Simple metadata endpoint describing entities, keys and enums to help external consumers.
    /// This is intentionally lightweight; expand as needed.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var assembly = typeof(Product).Assembly;
            var entityTypes = new[] { typeof(Brand), typeof(Category), typeof(Product), typeof(ProductAttribute), typeof(PriceList), typeof(ProductPrice), typeof(ProductRelation), typeof(ProductImage) };

            var entities = entityTypes.Select(t => new
            {
                Name = t.Name,
                Properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new { p.Name, Type = p.PropertyType.Name })
            });

            var enums = new[] { typeof(ProductStatus), typeof(AttributeDataType) }.Select(e => new
            {
                Name = e.Name,
                Values = Enum.GetNames(e)
            });

            return Ok(new { Entities = entities, Enums = enums });
        }
    }
}
