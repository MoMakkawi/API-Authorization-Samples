# ASP .NET Core Web API Authorization Implementation Samples :
Note: There are many ways to implement those ideas but I will show you mine I also implemented [Basic Authentication](https://github.com/MoMakkawi/API-Authentication-Samples?tab=readme-ov-file#basic-authentication).
## Policy Based Authorization:
there are a lot of similarities between Policy Based Authorization and Role-Based Authorization, most important things:
## Role-Based Authorization:
Most important things:
- I created [Roles.cs](https://github.com/MoMakkawi/API-Authorization-Samples/blob/master/Role%20Based%20Authorization/Entities/Roles.cs), a static class called which has roles as constant strings ```"ADMIN", "USER", "GUEST"```.
- I created a DB table to know every user's roles does he/she have, this table has a complex primary key UserId + Role.
```cs
public sealed class UserRole
{
    public int UserId { get; set; }
    public required string Role { get; set; }
}
```
- In [BasicAuthHandler.cs](https://github.com/MoMakkawi/API-Authorization-Samples/blob/master/Role%20Based%20Authorization/AuthenticationAndAuthorization/BasicAuthHandler.cs) you must add user roles as claims, eg:
```cs
// some code...
        // create user roles claims.
        var claims = await DbContext
            .Set<UserRole>()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => new Claim(ClaimTypes.Role, ur.Role))
            .ToListAsync();

        // add name id claim.
        claims.Add(new(ClaimTypes.NameIdentifier, user.Id.ToString()));
//some code...
```
- Finally in [Program.cs](https://github.com/MoMakkawi/API-Authorization-Samples/blob/master/Role%20Based%20Authorization/Program.cs):
```cs
//some code...
group.MapGet("/get-secret", 
    [Authorize(Roles = Roles.ADMIN)] // only for user that one of his roles is admin
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
//some code...
```

## Permission-Based Authorization:
Most important things:
- I used an enum for your permission, in some cases you will make permissions 
like ```Create, Read, Update, Delete ... ``` or like ``` post, get, delete, put, patch, header...``` . in my example, I make it like this
```cs
public enum Permission
{
    GetSecret = 1,
    GetHello = 2,
}
```
- I created a DB table to know every user's permissions does he/she have, this table has a complex primary key UserId + Permission.
```cs
public class UserPermission
{
    public int UserId { get; set; }
    public Permission Permission { get; set; }
}
```
- I made CheckPermissionAttribute an attribute to mark endpoints and make it able to check user permission.
```cs
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class CheckPermissionAttribute(Permission permission) : Attribute
{
    public readonly Permission Permission = permission;
}
```
- I made [PermissionFilter](https://github.com/MoMakkawi/API-Authorization-Samples/blob/master/Permission%20Based%20Authorization/AuthenticationAndAuthorization/PermissionFilter.cs) a filter to check if the user has permission to execute the requested endpoint.
```cs
public class PermissionFilter(BloggingContext DbContext) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var attributes = context.HttpContext
            .GetEndpoint()?
            .Metadata
            .GetOrderedMetadata<CheckPermissionAttribute>();

        if (attributes is null) return await next(context);

        var claimIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
        if (claimIdentity is not { IsAuthenticated: true })
        {
           context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return new ForbidResult();
        }

        var userId = int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var currentUserPermissions = await DbContext.Set<UserPermission>()
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission)
            .ToListAsync();

        var attributesPermissions = attributes.Select(a => a.Permission);
        var hasPermission = currentUserPermissions
            .Intersect(attributesPermissions)
            .Any();

        if (!hasPermission)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return new ForbidResult();
        }
        return await next(context);
    }
}
```

- Finally in [program.cs](https://github.com/MoMakkawi/API-Authorization-Samples/blob/master/Permission%20Based%20Authorization/Program.cs) :
```cs
// some code ...
var group = app.MapGroup("/api")
    .AddEndpointFilter<PermissionFilter>();

group.MapGet("/get-secret", [CheckPermission(Permission.GetSecret)] () => "Admin Secret!")
    .WithName("GetSecret")
    .WithOpenApi();

group.MapGet("/get-hello", () => "Hello, World !")
    .WithOpenApi()
    .WithName("GetHello")
    .WithMetadata
    (
        new CheckPermissionAttribute(Permission.GetSecret),
        new CheckPermissionAttribute(Permission.GetHello)
    );
// some code ...
```
