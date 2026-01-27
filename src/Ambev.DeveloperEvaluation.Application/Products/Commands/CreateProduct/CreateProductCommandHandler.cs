using Ambev.DeveloperEvaluation.Application.Products.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Products;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repo;

    public CreateProductCommandHandler(IProductRepository repo)
        => _repo = repo;

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken ct)
    {
        var externalId = request.ExternalId.Trim();

        var exists = await _repo.ExistsByExternalIdAsync(
            externalId,
            excludingId: null,
            ct);

        if (exists)
            throw new DomainException(
                $"Já existe um produto com externalId '{externalId}'."
            );

        var product = new Product(
            externalId,
            request.Name,
            request.Description,
            request.Price);

        await _repo.AddAsync(product, ct);

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
