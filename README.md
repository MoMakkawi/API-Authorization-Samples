# ASP .NET Core Web API Authorization Implementation Samples :
Note: There are many ways to implement those ideas but I will show you mine I also implemented [Basic Authentication](https://github.com/MoMakkawi/API-Authentication-Samples?tab=readme-ov-file#basic-authentication).
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
- I made PermissionFilter a filter to check if the user has permission to execute the requested endpoint.
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
