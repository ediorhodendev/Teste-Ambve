using Ambev.DeveloperEvaluation.Domain.Entities.Branches;
using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Branches;

public sealed class BranchRepository : IBranchRepository
{
    private readonly DefaultContext _ctx;

    public BranchRepository(DefaultContext ctx) => _ctx = ctx;

    public Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct)
        => _ctx.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Branch>> GetAllAsync(CancellationToken ct)
        => _ctx.Branches.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Branch?> GetByIdTrackedAsync(Guid id, CancellationToken ct)
        => _ctx.Branches.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? ignoringId, CancellationToken ct)
        => _ctx.Branches.AnyAsync(x => x.Name == name && (!ignoringId.HasValue || x.Id != ignoringId), ct);

    public async Task AddAsync(Branch branch, CancellationToken ct)
    {
        _ctx.Branches.Add(branch);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Branch branch, CancellationToken ct)
    {
        _ctx.Branches.Update(branch);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Branch branch, CancellationToken ct)
    {
        _ctx.Branches.Remove(branch);
        await _ctx.SaveChangesAsync(ct);
    }
}
