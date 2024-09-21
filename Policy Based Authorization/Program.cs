using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Policy_Based_Authorization.AuthenticationAndAuthorization;
using Policy_Based_Authorization.Data;

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
    .SeedBlogs();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();