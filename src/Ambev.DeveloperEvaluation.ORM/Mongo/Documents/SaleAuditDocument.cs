using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.ORM.Mongo.Documents;

public sealed class SaleAuditDocument
{
    [BsonId]
    public Guid Id { get; set; }

    public Guid SaleId { get; set; }

    public string EventType { get; set; } = default!;

    public DateTime OccurredAt { get; set; }

    
   
    public BsonDocument Payload { get; set; } = new();
}
