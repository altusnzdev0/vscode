using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMDM.Data;

namespace ProductMDM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            return Ok(items);
        }
    }
}
