using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCaching.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "products";

        public ProductsController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            // 1. Check whether products are already cached
            if (_cache.TryGetValue(CacheKey, out List<Product>? products))
            {
                return Ok(new { Source = "Cache", Data = products });
            }

            // 2. Prepare data when the cache is empty
            // In a real application, fetch this from your database
            products = new List<Product>
            {
                new Product(1, "Laptop", 55000),
                new Product(2, "Mouse", 500),
                new Product(3, "Keyboard", 1500)
            };

            // Configure cache expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(40))
                .SetSlidingExpiration(TimeSpan.FromSeconds(20));

            // 3. Store products with the configured options
            _cache.Set(CacheKey, products, cacheOptions);


            return Ok(new { Source = "Fresh data", Data = products });
        }

        [HttpDelete("cache")]
        public IActionResult ClearCache()
        {
            _cache.Remove(CacheKey);

            return Ok("Products cache cleared.");
        }
    }

    public record Product(int Id, string Name, decimal Price);
}
