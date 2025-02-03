using Trackr.Application;
using Trackr.Application.Interfaces;
using Trackr.Application.Services;
using Trackr.Infrastructure.Interfaces;
using Trackr.Infrastructure;
using Trackr.Domain.Interfaces;
using Trackr.Infrastructure.Repositories;
using Trackr.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Trackr.Domain.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

builder.Services.AddIdentity<User, IdentityRole>()  
    .AddEntityFrameworkStores<AppDbContext>()     
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(SpotifyMapProfile), typeof(MappingProfile));

builder.Services.AddHttpClient<SpotifyClient>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlaybackService, PlaybackService>();
builder.Services.AddScoped<IClient, SpotifyClient>();

builder.Services.AddControllers();
//Repo and db context
//Auth

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
