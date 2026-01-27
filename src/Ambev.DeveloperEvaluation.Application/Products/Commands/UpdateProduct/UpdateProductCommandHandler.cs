using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;
    public UpdateProductCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.Id, ct);
        if (product is null) throw new KeyNotFoundException("Produto não encontrado.");

        var exists = await _repo.ExistsByExternalIdAsync(request.ExternalId.Trim(), excludingId: request.Id, ct);
        if (exists) throw new ArgumentException("Já existe um produto com este ExternalId.");

        product.Update(
            request.ExternalId,
            request.Name,
            request.Description,
            request.Price,
            request.IsActive);

        await _repo.UpdateAsync(product, ct);

        return new ProductDto
        {
            Id = product.Id,
            ExternalId = product.ExternalId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
