using Ambev.DeveloperEvaluation.Domain.Entities.Products;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Product>> GetAllAsync(CancellationToken ct);

    Task<bool> ExistsByExternalIdAsync(
        string externalId,
        Guid? excludingId,
        CancellationToken ct);

    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Product product, CancellationToken ct);
}
