namespace Ambev.DeveloperEvaluation.Domain.Entities.Products;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string ExternalId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected Product() { }

    public Product(string externalId, string name, string? description, decimal price)
    {
        SetExternalId(externalId);
        SetName(name);
        SetDescription(description);
        SetPrice(price);

        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public void Update(string externalId, string name, string? description, decimal price, bool isActive)
    {
        SetExternalId(externalId);
        SetName(name);
        SetDescription(description);
        SetPrice(price);

        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("ExternalId é obrigatório.");
        ExternalId = externalId.Trim();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do produto é obrigatório.");
        Name = name.Trim();
    }

    private void SetDescription(string? description)
        => Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    private void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Preço deve ser maior que zero.");
        Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }
}
