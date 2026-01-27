namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Dtos;

public sealed class UpdateProductRequestDto
{
    public string ExternalId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; } = true;
}

