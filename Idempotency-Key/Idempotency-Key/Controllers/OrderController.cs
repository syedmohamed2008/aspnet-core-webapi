using Idempotency_Key.Data;
using Idempotency_Key.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Idempotency_Key.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromHeader(Name = "Idempotency-Key")] string? key,
            [FromBody] CreateOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > 100)
                return BadRequest("Valid Idempotency-Key header is required.");

            if (string.IsNullOrWhiteSpace(request.ProductName))
                return BadRequest("ProductName is required.");

            // Check whether this key was processed earlier.
            var existingRecord = await _db.IdempotencyRecords
                                .AsNoTracking()
                                .SingleOrDefaultAsync(x => x.IdempotencyKey == key);

            if (existingRecord is not null)
                return SavedResponse(existingRecord);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = request.ProductName
            };

            var responseBody = JsonSerializer.Serialize(new
            {
                Message = "Order created.",
                OrderId = order.Id,
                order.ProductName
            });

            var record = new IdempotencyRecord
            {
                IdempotencyKey = key,
                ResponseBody = responseBody,
                StatusCode = StatusCodes.Status201Created,
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            _db.IdempotencyRecords.Add(record);

            try
            {
                // Saves the order AND its idempotency record together.
                await _db.SaveChangesAsync();

                return SavedResponse(record);
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sqlEx
                      && sqlEx.Number is 2601 or 2627)
            {
                // Another request may have saved the same key while
                // this request was running.
                var savedRecord = await _db.IdempotencyRecords
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.IdempotencyKey == key);

                if (savedRecord is null)
                    throw;

                return SavedResponse(savedRecord);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _db.Orders
                .AsNoTracking()
                .ToListAsync());
        }

        private static ContentResult SavedResponse(
            IdempotencyRecord record)
        {
            return new ContentResult
            {
                Content = record.ResponseBody,
                ContentType = "application/json",
                StatusCode = record.StatusCode
            };
        }
    }

    public record CreateOrderRequest(string ProductName);
}
