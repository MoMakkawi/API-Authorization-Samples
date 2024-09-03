using Permission_Based_Authorization.Entities;

namespace Permission_Based_Authorization.Middleware;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CheckPermissionAttribute(Permission permission) : Attribute
{
    public readonly Permission Permission = permission;
}