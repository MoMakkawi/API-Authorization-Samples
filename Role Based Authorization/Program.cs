using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Role_Based_Authorization.AuthenticationAndAuthorization;
using Role_Based_Authorization.Data;
using Role_Based_Authorization.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);
builder.Services.AddAuthorization();
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
    .SeedBlogs()
    .SeedUserRoles();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var group = app.MapGroup("/api");

group.MapGet("/get-secret", 
    [Authorize(Roles = Roles.ADMIN)] // // only for user that one of his roles is admin
    () => "Admin Secret!")
    .WithName("GetSecret")
    .WithOpenApi();

group.MapGet("/get-welcome",
    [Authorize(Roles = Roles.ADMIN)] // only for user that admin and user at same time
    [Authorize(Roles = Roles.USER)]
    () => "Hello, World !")
    .WithOpenApi()
    .WithName("GetWelcome");

group.MapGet("/get-hello",
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.USER}")] // admin or user
    () => "Hello, World !")
    .WithOpenApi()
    .WithName("GetHello");

app.Run();