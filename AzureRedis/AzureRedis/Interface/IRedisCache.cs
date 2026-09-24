namespace AzureRedis.Interface
{
    public interface IRedisCache
    {
        Task<T?> GetAsync<T>(string key) where T : class;

        Task SetAsync<T>(string key, T value, TimeSpan expiry);

        Task<bool> DeleteAsync(string key);
    }
}
