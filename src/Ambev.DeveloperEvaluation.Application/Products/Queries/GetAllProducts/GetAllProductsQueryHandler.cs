using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repo;
    public GetAllProductsQueryHandler(IProductRepository repo) => _repo = repo;

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list.Select(p => new ProductDto
        {
            Id = p.Id,
            ExternalId = p.ExternalId,
            Name = p.Name,
            Description = p.Description,
            IsActive = p.IsActive
        }).ToList();
    }
}
