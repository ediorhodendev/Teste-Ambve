namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed class ExternalIdentity
{
    public string ExternalId { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    private ExternalIdentity() { } 

    public ExternalIdentity(string externalId, string description)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("ExternalId cannot be empty.", nameof(externalId));

        ExternalId = externalId.Trim();
        Description = (description ?? string.Empty).Trim();
    }

    public void UpdateDescription(string description)
        => Description = (description ?? string.Empty).Trim();
}
