using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string ExternalId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive
) : IRequest<ProductDto>;
