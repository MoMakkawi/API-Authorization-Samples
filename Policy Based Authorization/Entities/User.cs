namespace Policy_Based_Authorization.Entities;

public sealed class User
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
    public required DateOnly Birthday { get; set; }
    public bool IsPremium { get; set; } 
}
