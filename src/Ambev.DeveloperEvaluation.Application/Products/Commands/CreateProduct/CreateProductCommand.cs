using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string ExternalId,
    string Name,
    string? Description,
    decimal Price
) : IRequest<ProductDto>;
