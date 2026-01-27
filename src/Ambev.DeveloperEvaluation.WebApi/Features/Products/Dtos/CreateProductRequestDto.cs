namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Dtos;

public sealed class CreateProductRequestDto
{
    public string ExternalId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public decimal Price { get; init; }
}
