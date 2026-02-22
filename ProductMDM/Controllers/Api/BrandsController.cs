using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BrandsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _db.Brands.OrderBy(b => b.Name).ToListAsync();
            return Ok(items);
        }
    }
}
