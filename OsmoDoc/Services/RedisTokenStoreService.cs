using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace OsmoDoc.Services;

public class RedisTokenStoreService : IRedisTokenStoreService
{
    private readonly IDatabase _db;
    private const string KeyPrefix = "valid_token:";

    public RedisTokenStoreService(IConnectionMultiplexer redis)
    {
        this._db = redis.GetDatabase();
    }

    public Task StoreTokenAsync(string token, string email)
    {
        return this._db.StringSetAsync($"{KeyPrefix}{token}", JsonConvert.SerializeObject(new {
            issuedTo = email,
            issuedAt = DateTime.UtcNow
        }));
    }

    public Task<bool> IsTokenValidAsync(string token)
    {
        return this._db.KeyExistsAsync($"{KeyPrefix}{token}");
    }

    public Task RevokeTokenAsync(string token)
    {
        return this._db.KeyDeleteAsync($"{KeyPrefix}{token}");
    }
}