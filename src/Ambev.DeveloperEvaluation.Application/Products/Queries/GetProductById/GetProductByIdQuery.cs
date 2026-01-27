using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
