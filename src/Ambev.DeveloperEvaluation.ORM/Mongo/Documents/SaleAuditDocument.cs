using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.ORM.Mongo.Documents;

public class SaleAuditDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid SaleId { get; set; }

    public string EventType { get; set; } = default!;
    public DateTime OccurredAt { get; set; }

    public object Payload { get; set; } = default!;
}
