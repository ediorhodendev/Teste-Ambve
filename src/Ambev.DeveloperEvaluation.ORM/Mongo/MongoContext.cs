using Ambev.DeveloperEvaluation.ORM.Mongo.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Mongo;

public sealed class MongoContext
{
    private static bool _serializersRegistered;
    private static readonly object _lock = new();

    private readonly IMongoDatabase _db;
    private readonly MongoSettings _settings;

    public MongoContext(IOptions<MongoSettings> options)
    {
        _settings = options.Value;

        RegisterGuidSerializersOnce();

        var client = new MongoClient(_settings.ConnectionString);
        _db = client.GetDatabase(_settings.Database);
    }

    private static void RegisterGuidSerializersOnce()
    {
        if (_serializersRegistered) return;

        lock (_lock)
        {
            if (_serializersRegistered) return;

            // Driver atual: registra Guid como STANDARD
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));

            _serializersRegistered = true;
        }
    }

    public IMongoCollection<SaleAuditDocument> SaleAuditCollection()
        => _db.GetCollection<SaleAuditDocument>(_settings.AuditCollection);
}
