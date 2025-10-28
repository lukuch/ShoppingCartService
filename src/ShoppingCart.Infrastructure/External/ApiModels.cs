namespace ShoppingCart.Infrastructure.External;

public record ProductApiResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public CategoryApiResponse Category { get; init; } = new();
    public string[] Images { get; init; } = Array.Empty<string>();
}

public record CategoryApiResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
}

