namespace Role_Based_Authorization.Entities;

public sealed class Blog
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
}