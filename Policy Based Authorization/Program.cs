using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Policy_Based_Authorization.AuthenticationAndAuthorization;
using Policy_Based_Authorization.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Subscription", builder => builder.AddRequirements(new SubscriptionAuthorizationRequirement()))
    .AddPolicy("AgePlus18", builder =>
    {
        builder.RequireAssertion(context =>
        {
            if (context.User.FindFirstValue("Birthday") is not string birthday) return false;

            var nowDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var birthdayDate = DateOnly.Parse(birthday);

            var userAge = nowDate.Year - birthdayDate.Year;

            if (nowDate < birthdayDate.AddYears(userAge))
                userAge--;

            return userAge >= 18;
        });
    });

builder.Services.AddScoped<IAuthorizationHandler, SubscriptionAuthorizationHandler>();

builder.Services.AddSwaggerGen(c =>
{
    // Add basic authentication scheme to Swagger document
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        Description = "Basic authentication header"
    });

    // Add a requirement to use the defined authentication scheme in all operations
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Service
builder.Services.AddDbContext<BloggingContext>(options =>
    options.UseInMemoryDatabase("BloggingDB"));

var app = builder.Build();

// Seed data
app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<BloggingContext>()
    .SeedUsers()
    .SeedBlogs();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var group = app.MapGroup("/api");

// without policies.
group.MapGet("/get-hello", () => "Hello, World !")
    .WithName("GetHello")
    .AllowAnonymous();

// with policy written in this file
group.MapGet("/get-plus18", () => "Hello, World , you are +18 !")
    .WithName("GetPlus18")
    .RequireAuthorization("AgePlus18");

// with policy written in separated file
group.MapGet("/get-premium", () => "Hello, World , you are premium!")
    .WithName("GetPremium")
    .RequireAuthorization("Subscription");

app.Run();