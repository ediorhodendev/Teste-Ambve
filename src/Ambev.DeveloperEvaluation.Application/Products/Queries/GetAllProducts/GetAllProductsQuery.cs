using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery() : IRequest<List<ProductDto>>;
