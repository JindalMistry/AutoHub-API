using AutoHub.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AutoHub.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedValue = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedValue)) return default;

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task<long?> GetLongAsync(string key)
    {
        var db = _redis.GetDatabase();

        var value = await db.StringGetAsync(key);

        if (!value.HasValue)
            return null;

        return (long)value;
    }

    public async Task<long> IncrementAsync(string key)
    {
        var db = _redis.GetDatabase();

        return await db.StringIncrementAsync(key);
    }

    public async Task<long> DecrementAsync(string key)
    {
        var db = _redis.GetDatabase();

        return await db.StringDecrementAsync(key);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
            };

        var serialized = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            serialized,
            options);
    }
}