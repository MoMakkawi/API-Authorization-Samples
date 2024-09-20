namespace Permission_Based_Authorization.Entities;

internal sealed class Blog
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
}