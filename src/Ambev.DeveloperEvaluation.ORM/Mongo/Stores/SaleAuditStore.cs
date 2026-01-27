using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.ORM.Mongo.Documents;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Mongo.Stores;

public sealed class SaleAuditStore : ISaleAuditStore
{
    private readonly IMongoCollection<SaleAuditDocument> _collection;

    public SaleAuditStore(MongoContext context)
    {
        _collection = context.SaleAuditCollection();
    }

    public async Task AppendAsync(
        string eventType,
        Guid saleId,
        object payload,
        CancellationToken ct)
    {
        var document = new SaleAuditDocument
        {
            SaleId = saleId,
            EventType = eventType,
            OccurredAt = DateTime.UtcNow,
            Payload = payload 
        };

        await _collection.InsertOneAsync(document, cancellationToken: ct);
    }
}
