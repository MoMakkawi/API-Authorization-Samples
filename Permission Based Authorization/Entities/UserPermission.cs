namespace Permission_Based_Authorization.Entities;

public class UserPermission
{
    public int UserId { get; set; }
    public Permission Permission { get; set; }
}
