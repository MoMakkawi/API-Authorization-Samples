namespace Role_Based_Authorization.Entities;

public sealed class UserRole
{
    public int Id { get; set; }
    public required string Role { get; set; }
}