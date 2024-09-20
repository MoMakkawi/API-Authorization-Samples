namespace Permission_Based_Authorization.Entities;

internal sealed class UserPermission
{
    public int UserId { get; set; }
    public Permission Permission { get; set; }
}
