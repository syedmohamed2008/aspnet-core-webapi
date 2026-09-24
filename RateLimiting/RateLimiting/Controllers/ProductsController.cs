using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("api/products")]
    [EnableRateLimiting("FixedPolicy")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[] { "Laptop", "Mouse", "Keyboard" });
        }
    }
}
