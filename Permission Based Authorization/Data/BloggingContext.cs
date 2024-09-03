using Microsoft.EntityFrameworkCore;

using Permission_Based_Authorization.Entities;

namespace Permission_Based_Authorization.Data;

public class BloggingContext(DbContextOptions<BloggingContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseInMemoryDatabase("BloggingDB");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPermission>()
            .HasKey(up => new { up.UserId, up.Permission });

        base.OnModelCreating(modelBuilder);
    }
}