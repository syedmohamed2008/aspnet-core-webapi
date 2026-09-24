using AzureRedis.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace AzureRedis.Services
{
    public class RedisCache : IRedisCache
    {
        private readonly IDatabase _db;

        public RedisCache(IConnectionMultiplexer connection)
        {
            // Gets an object used to execute Redis commands.
            // Reuses the connection managed by IConnectionMultiplexer.
            _db = connection.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            // GET: Read the JSON string stored under this key.
            RedisValue value = await _db.StringGetAsync(key);

            // The key may not exist or may have expired.
            if (value.IsNullOrEmpty)
                return null;

            // Convert JSON back into the requested C# object.
            return JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            // Convert the C# object into a JSON string.
            string json = JsonSerializer.Serialize(value);

            // SET: Create or overwrite the key with an expiration.
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            // DELETE: true if removed; false if the key did not exist.
            return await _db.KeyDeleteAsync(key);
        }
    }
}
