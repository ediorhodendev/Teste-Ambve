using Ambev.DeveloperEvaluation.Domain.Entities.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Products;

public sealed class ProductRepository : IProductRepository
{
    private readonly DefaultContext _ctx;

    public ProductRepository(DefaultContext ctx) => _ctx = ctx;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _ctx.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<Product>> GetAllAsync(CancellationToken ct)
        => await _ctx.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

   
    public async Task<bool> ExistsByExternalIdAsync(
        string externalId,
        Guid? excludingId,
        CancellationToken ct)
        => await _ctx.Products
            .AsNoTracking()
            .AnyAsync(x =>
                x.ExternalId == externalId &&
                (excludingId == null || x.Id != excludingId),
                ct);

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Product product, CancellationToken ct)
    {
        _ctx.Products.Remove(product);
        await _ctx.SaveChangesAsync(ct);
    }
}
