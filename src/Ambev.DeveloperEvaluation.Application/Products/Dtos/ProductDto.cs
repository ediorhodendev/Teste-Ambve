namespace Ambev.DeveloperEvaluation.Application.Products.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
