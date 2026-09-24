using AzureRedis.Interface;
using AzureRedis.Model;
using Microsoft.AspNetCore.Mvc;

namespace AzureRedis.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IRedisCache _redisCache;

    // All requests use this same Redis key.
    private const string CacheKey = "demo:products:all";

    public ProductsController(IRedisCache redisCache)
    {
        _redisCache = redisCache;
    }

    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        // 1. GET: Check whether products are already cached.
        var products =
            await _redisCache.GetAsync<List<Product>>(CacheKey);

        if (products is not null)
        {
            // Cache hit: return data retrieved from Redis.
            return Ok(new
            {
                Source = "Redis Cache",
                Products = products
            });
        }

        // 2. Cache miss: use hard-coded products.
        // In a real application, fetch these from your database.
        products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 55000m },
            new Product { Id = 2, Name = "Mouse", Price = 750m },
            new Product { Id = 3, Name = "Keyboard", Price = 1500m }
        };

        // 3. SET: Save products in Redis for five minutes.
        await _redisCache.SetAsync(CacheKey, products, TimeSpan.FromMinutes(5));

        // 4. Return the products.
        return Ok(new
        {
            Source = "Hard-coded List",
            Products = products
        });
    }

    // DELETE /api/products/cache
    [HttpDelete("cache")]
    public async Task<IActionResult> DeleteProductsCache()
    {
        // DELETE: Remove only the cached products.
        // This does not change the hard-coded list.
        bool deleted = await _redisCache.DeleteAsync(CacheKey);

        return Ok(new
        {
            Message = deleted
                ? "Products cache deleted."
                : "Products cache does not exist."
        });
    }
}