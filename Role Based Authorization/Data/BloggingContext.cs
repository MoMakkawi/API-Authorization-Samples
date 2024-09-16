using Microsoft.EntityFrameworkCore;

using Role_Based_Authorization.Entities;

namespace Role_Based_Authorization.Data;

public class BloggingContext(DbContextOptions<BloggingContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.Role });

        base.OnModelCreating(modelBuilder);
    }
}