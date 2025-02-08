//using StackExchange.Redis;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Infrastructure;
using Xunit;

public class RedisCacheTests
{
    //private readonly CacheService _cacheService;
    private readonly IDatabase _database;
    private readonly ConnectionMultiplexer _redis;
    private readonly ICache _cache;

    public RedisCacheTests()
    {
        var muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "", 15459 } },
                User = "default",
                Password = ""
            }
        ); 

        _cache = new RedisCacheClient(muxer);

        /*//  (local or cloud)
        string redisConnectionString = "localhost:6379";

        _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        
        _cacheService = new CacheService(_database);*/

    }

    [Fact]
    public async Task SetAsync_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        string key = "ionzjdbcchay23y2bhxzcghich--12";
        string value = "zasdadiwivoicvjkmkfgnd123c";
        //RedisValue jsonData = System.Text.Json.JsonSerializer.Serialize(value);
        TimeSpan timeSpan = DateTime.UtcNow.AddHours(1) - DateTime.UtcNow;

        // Act
        await _cache.SetAsync(key, value, timeSpan);
        string retrievedValue = await _cache.GetAsync<string>(key);

        // Assert
        Assert.NotNull(retrievedValue);
        Assert.Equal("zasdadiwivoicvjkmkfgnd123c", retrievedValue);

    }

    /*[Fact]
    public async Task SetAsync_ShouldThrowException_WhenSetFails()
    {
        // Arrange
        string key = "invalid:key";  // 
        var value = new { Name = "Bob", Age = 30 };
        TimeSpan expiry = TimeSpan.FromMinutes(5);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _cacheService.SetAsync(key, value, expiry));
    }*/
}
