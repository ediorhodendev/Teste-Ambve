namespace Ambev.DeveloperEvaluation.ORM.Mongo;

public sealed class MongoSettings
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; set; } = default!;
    public string AuditCollection { get; set; } = "sale_events";
}

