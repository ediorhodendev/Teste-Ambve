using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repo;
    public DeleteProductCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.Id, ct);
        if (product is null) throw new KeyNotFoundException("Produto não encontrado.");

        await _repo.DeleteAsync(product, ct);
    }
}
