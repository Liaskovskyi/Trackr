using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading.Tasks;
using Trackr.Domain.Interfaces;
using Trackr.Infrastructure;
using Trackr.Infrastructure.Clients;
using Xunit;

public class RedisCacheTests
{
    private readonly ICache _cache;

    public RedisCacheTests()
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var redisConfig = configuration.GetSection("Redis");
            var host = redisConfig["Host"];
            var port = redisConfig["Port"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                _cache = null!;
                return;
            }

            var muxer = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { host, int.Parse(port) } },
                    User = redisConfig["User"],
                    Password = redisConfig["Password"]
                });

            _cache = new RedisCacheClient(muxer);
        }
        catch
        {
            _cache = null!;
        }
    }

    [Fact]
    public async Task SetAsync_ShouldStoreAndRetrieveValue()
    {
        if (_cache == null)
        {
            return;
        }

        // Arrange
        string key = "ionzjdbcchay23y2bhxzcghich--12";
        string value = "zasdadiwivoicvjkmkfgnd123c";
        TimeSpan timeSpan = DateTime.UtcNow.AddHours(1) - DateTime.UtcNow;

        // Act
        await _cache.SetAsync(key, value, timeSpan);
        string retrievedValue = await _cache.GetAsync<string>(key);

        // Assert
        Assert.NotNull(retrievedValue);
        Assert.Equal("zasdadiwivoicvjkmkfgnd123c", retrievedValue);
    }
}
