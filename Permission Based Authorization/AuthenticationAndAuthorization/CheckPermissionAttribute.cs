using Permission_Based_Authorization.Entities;

namespace Permission_Based_Authorization.AuthenticationAndAuthorization;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class CheckPermissionAttribute(Permission permission) : Attribute
{
    public readonly Permission Permission = permission;
}