using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Domain.Models;

namespace Trackr.Infrastructure.Clients
{
    public class RedisCacheClient : ICache
    {
        private readonly StackExchange.Redis.IDatabase _database;

        public RedisCacheClient(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            RedisValue data = await _database.StringGetAsync(key);
            return data.HasValue ? System.Text.Json.JsonSerializer.Deserialize<T>(data) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiriry)
        {
            RedisValue jsonData = System.Text.Json.JsonSerializer.Serialize(value);
            bool result = await _database.StringSetAsync(key, jsonData, expiriry);
            if (!result) throw new Exception("An error occured while caching an access token.");
        }
    }
}
