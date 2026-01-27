using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest;
