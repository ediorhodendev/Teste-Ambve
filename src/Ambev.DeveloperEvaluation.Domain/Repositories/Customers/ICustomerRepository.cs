using Ambev.DeveloperEvaluation.Domain.Entities.Customers;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Customers;

public interface ICustomerRepository
{
    // Reads
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Customer>> GetAllAsync(CancellationToken ct);

    // Writes (tracked)
    Task<Customer?> GetByIdTrackedAsync(Guid id, CancellationToken ct);

    // Uniqueness checks
    Task<bool> ExistsByDocumentAsync(string document, Guid? ignoringId, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, Guid? ignoringId, CancellationToken ct);

    // CRUD
    Task AddAsync(Customer customer, CancellationToken ct);
    Task UpdateAsync(Customer customer, CancellationToken ct);
    Task DeleteAsync(Customer customer, CancellationToken ct);
}
