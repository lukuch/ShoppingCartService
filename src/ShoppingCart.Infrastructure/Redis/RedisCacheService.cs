using System.Text.Json;
using ShoppingCart.Application.Interfaces;
using StackExchange.Redis;

namespace ShoppingCart.Infrastructure.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var serializedValue = JsonSerializer.Serialize(value);
        var expiry = expiration ?? DefaultExpiration;

        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        return await _database.KeyExistsAsync(key);
    }
}
