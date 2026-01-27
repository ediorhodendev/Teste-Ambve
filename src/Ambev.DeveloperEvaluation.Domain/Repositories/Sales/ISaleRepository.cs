using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;

public interface ISaleRepository
{
    Task AddAsync(Sale sale, CancellationToken ct);
    Task UpdateAsync(Sale sale, CancellationToken ct);

    Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct);

    
    Task<Sale?> GetByIdWithItemsAsync(Guid id, CancellationToken ct);

    Task<(IReadOnlyList<Sale> Items, int Total)> GetPagedAsync(
       int page,
       int pageSize,
       string? status,
       DateTime? from,
       DateTime? to,
       string? sortBy,
       string? sortDir,
       CancellationToken ct);
    Task<bool> ExistsBySaleNumberAsync(string saleNumber, Guid? excludingId, CancellationToken ct);

}
