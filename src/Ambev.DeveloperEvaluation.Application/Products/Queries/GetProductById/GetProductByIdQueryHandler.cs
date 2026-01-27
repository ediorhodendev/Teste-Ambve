using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repo;
    public GetProductByIdQueryHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(request.Id, ct);
        if (p is null) return null;

        return new ProductDto
        {
            Id = p.Id,
            ExternalId = p.ExternalId,
            Name = p.Name,
            Description = p.Description,
            IsActive = p.IsActive
        };
    }
}
