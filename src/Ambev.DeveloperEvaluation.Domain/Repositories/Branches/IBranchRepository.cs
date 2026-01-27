using Ambev.DeveloperEvaluation.Domain.Entities.Branches;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Branches;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Branch>> GetAllAsync(CancellationToken ct);

    Task<Branch?> GetByIdTrackedAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsByNameAsync(string name, Guid? ignoringId, CancellationToken ct);

    Task AddAsync(Branch branch, CancellationToken ct);
    Task UpdateAsync(Branch branch, CancellationToken ct);
    Task DeleteAsync(Branch branch, CancellationToken ct);
}
