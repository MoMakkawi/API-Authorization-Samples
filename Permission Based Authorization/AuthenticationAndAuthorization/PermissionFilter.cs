using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Permission_Based_Authorization.Data;
using Permission_Based_Authorization.Entities;

namespace Permission_Based_Authorization.AuthenticationAndAuthorization;

internal class PermissionFilter(BloggingContext DbContext) : IEndpointFilter
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