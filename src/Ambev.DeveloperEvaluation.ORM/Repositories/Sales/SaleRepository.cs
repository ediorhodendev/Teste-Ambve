using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Sales;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _ctx;

    public SaleRepository(DefaultContext ctx) => _ctx = ctx;

    public async Task AddAsync(Sale sale, CancellationToken ct)
    {
        _ctx.Sales.Add(sale);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken ct)
    {
        _ctx.Sales.Update(sale);
        await _ctx.SaveChangesAsync(ct);
    }

    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct)
        => _ctx.Sales.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Sale?> GetByIdWithItemsAsync(Guid id, CancellationToken ct)
        => _ctx.Sales.Include(x => x.Items)
                     .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<Sale> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? status,
        DateTime? from,
        DateTime? to,
        string? sortBy,
        string? sortDir,
        CancellationToken ct)
    {
        var query = _ctx.Sales
            .AsNoTracking()
            .Include(x => x.Items)
            .AsQueryable();

        // filtros
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<SaleStatus>(status, true, out var st))
        {
            query = query.Where(x => x.Status == st);
        }

        if (from.HasValue)
            query = query.Where(x => x.SaleDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.SaleDate <= to.Value);

        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sortBy ?? "SaleDate").ToLowerInvariant() switch
        {
            "totalamount" => desc ? query.OrderByDescending(x => x.TotalAmount)
                                  : query.OrderBy(x => x.TotalAmount),

            "salenumber" => desc ? query.OrderByDescending(x => x.SaleNumber)
                                  : query.OrderBy(x => x.SaleNumber),

            _ => desc ? query.OrderByDescending(x => x.SaleDate)
                      : query.OrderBy(x => x.SaleDate),
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<bool> ExistsBySaleNumberAsync(string saleNumber, Guid? excludingId, CancellationToken ct)
    {
        saleNumber = saleNumber.Trim();

        return await _ctx.Sales
            .AsNoTracking()
            .AnyAsync(s =>
                s.SaleNumber == saleNumber &&
                (excludingId == null || s.Id != excludingId),
                ct);
    }
}
