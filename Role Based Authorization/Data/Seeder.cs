﻿using Role_Based_Authorization.Entities;

namespace Role_Based_Authorization.Data;

public static class Seeder
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
                    Password = "Password"
                },
                new User
                {
                    Id = 2,
                    Email = "MoMakkawi@mail.com",
                    FirstName = "Mo",
                    LastName = "Makkawi",
                    UserName = "MoMakkawi",
                    Password = "Password123"
                }
            );

            context.SaveChanges();
        }

        return context;
    }
    public static BloggingContext SeedUserRoles(this BloggingContext context)
    {
        if (!context.Set<UserRole>().Any())
        {
            context.Set<UserRole>().AddRange(
                new UserRole { UserId = 1, Role = Roles.ADMIN },
                new UserRole { UserId = 2, Role = Roles.USER }
            );
            context.SaveChanges();
        }

        return context;
    }
}