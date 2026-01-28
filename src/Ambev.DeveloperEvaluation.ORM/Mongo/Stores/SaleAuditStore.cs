using System.Text.Json;
using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.ORM.Mongo.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Mongo.Stores;

public sealed class SaleAuditStore : ISaleAuditStore
{
    private readonly IMongoCollection<SaleAuditDocument> _collection;
    private readonly ILogger<SaleAuditStore> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SaleAuditStore(MongoContext context, ILogger<SaleAuditStore> logger)
    {
        _collection = context.SaleAuditCollection();
        _logger = logger;
    }

    public async Task AppendAsync(string eventType, Guid saleId, object payload, CancellationToken ct)
    {
        var document = new SaleAuditDocument
        {
            Id = Guid.NewGuid(),
            SaleId = saleId,
            EventType = eventType,
            OccurredAt = DateTime.UtcNow,
            Payload = ToBsonSafe(payload)
        };

        try
        {
            await _collection.InsertOneAsync(document, cancellationToken: ct);
        }
        catch (MongoException ex)
        {
            
            _logger.LogWarning(ex, "Mongo unavailable. Audit skipped. eventType={EventType} saleId={SaleId}", eventType, saleId);
        }
    }

    private BsonDocument ToBsonSafe(object payload)
    {
        if (payload is BsonDocument bson) return bson;

        
        try
        {
            return payload.ToBsonDocument();
        }
        catch (Exception ex) when (ex is BsonSerializationException or InvalidOperationException)
        {
            
            try
            {
                var json = JsonSerializer.Serialize(payload, JsonOptions);
                return BsonDocument.Parse(json);
            }
            catch (Exception ex2)
            {
                _logger.LogWarning(ex2, "Failed to serialize audit payload. payloadType={PayloadType}", payload?.GetType().FullName);
                return new BsonDocument
                {
                    { "serializationError", ex2.Message },
                    { "payloadType", payload?.GetType().FullName ?? "null" }
                };
            }
        }
    }
}
