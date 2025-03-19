using Trackr.Application;
using Trackr.Application.Interfaces;
using Trackr.Application.Services;
using Trackr.Infrastructure;
using Trackr.Domain.Interfaces;
using Trackr.Infrastructure.Repositories;
using Trackr.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Trackr.Api.Mappers;
using Microsoft.Extensions.Options;
using Trackr.Api.Configuration;
using StackExchange.Redis;
using System.Security.Principal;
using System.Security.Claims;
using Trackr.Domain.Models.Database;
using Trackr.Infrastructure.Clients;
using Trackr.Infrastructure.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();


builder.Services.AddDbContext<AppDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddIdentity<User, IdentityRole>()  
    .AddEntityFrameworkStores<AppDbContext>()     
    .AddDefaultTokenProviders();

//Caching
var redisConfig = builder.Configuration.GetSection("Redis");
var muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { redisConfig["Host"]!, int.Parse(redisConfig["Port"]!) } },
                User = redisConfig["User"],
                Password = redisConfig["Password"]
            });

builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);
builder.Services.AddScoped<ICache, RedisCacheClient>();

var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IMessageQueue, RabbitMQClient>(provider=> 
    new RabbitMQClient(rabbitMQSettings["HostName"]!, rabbitMQSettings["UserName"]!, rabbitMQSettings["Password"]!));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerServices();

builder.Services.AddAutoMapper(typeof(SpotifyMapProfile), typeof(MappingProfile), typeof(IdentityMapProfile));

builder.Services.AddHttpClient<SpotifyClient>();

//builder.Services.AddScoped<IPrincipal, ClaimsPrincipal>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IListenedRepository, ListenedRepository>();
builder.Services.AddScoped<IPlaybackService, PlaybackService>();
builder.Services.AddScoped<IClient, SpotifyClient>();
builder.Services.AddScoped<ITrackService, TrackService>();

builder.Services.AddSingleton<IJwtGenerator, JWTGenerator>();
builder.Services.AddHostedService<MessageConsumerService>();

builder.Services.AddControllers();



var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
