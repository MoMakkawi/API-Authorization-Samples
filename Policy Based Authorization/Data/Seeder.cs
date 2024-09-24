using Policy_Based_Authorization.Entities;

namespace Policy_Based_Authorization.Data;

internal static class Seeder
{
    public static BloggingContext SeedBlogs(this BloggingContext context)
    {
        if (!context.Blogs.Any())
        {
            context.Blogs.AddRange(
                new Blog { Name = "Tech Blog", Description = "A blog about the latest in tech", Url = "https://techblog.com" },
                new Blog { Name = "Food Blog", Description = "Delicious recipes and cooking tips", Url = "https://foodblog.com" },
                new Blog { Name = "Travel Blog", Description = "Travel tips and destination guides", Url = "https://travelblog.com" }
            );

            context.SaveChanges();
        }

        return context;
    }
    public static BloggingContext SeedUsers(this BloggingContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Email = "Admin@mail.com",
                    FirstName = "Admin FN",
                    LastName = "Admin LN",
                    UserName = "Admin",
                    Password = "Password",
                    Birthday = DateOnly.Parse("2000-02-12")
                },
                new User
                {
                    Id = 2,
                    Email = "MoMakkawi@Hotmail.com",
                    FirstName = "Mo",
                    LastName = "Makkawi",
                    UserName = "MoMakkawi",
                    Password = "Password123",
                    Birthday = DateOnly.MaxValue,
                }
            );

            context.SaveChanges();
        }

        return context;
    }
}