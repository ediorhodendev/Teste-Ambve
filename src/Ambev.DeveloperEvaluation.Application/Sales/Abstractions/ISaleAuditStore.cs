
namespace Ambev.DeveloperEvaluation.Application.Sales.Abstractions;

public interface ISaleAuditStore
{
    Task AppendAsync(string eventType, Guid saleId, object payload, CancellationToken ct);
}
