using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories.Customers;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly DefaultContext _ctx;

    public CustomerRepository(DefaultContext ctx) => _ctx = ctx;

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => _ctx.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Customer>> GetAllAsync(CancellationToken ct)
        => _ctx.Customers.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<Customer?> GetByIdTrackedAsync(Guid id, CancellationToken ct)
        => _ctx.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByDocumentAsync(string document, Guid? ignoringId, CancellationToken ct)
        => _ctx.Customers.AnyAsync(x => x.Document == document && (!ignoringId.HasValue || x.Id != ignoringId), ct);

    public Task<bool> ExistsByEmailAsync(string email, Guid? ignoringId, CancellationToken ct)
        => _ctx.Customers.AnyAsync(x => x.Email == email && (!ignoringId.HasValue || x.Id != ignoringId), ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Add(customer);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Update(customer);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken ct)
    {
        _ctx.Customers.Remove(customer);
        await _ctx.SaveChangesAsync(ct);
    }
}
