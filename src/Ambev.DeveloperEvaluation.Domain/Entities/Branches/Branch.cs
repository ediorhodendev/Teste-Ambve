using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Branches;

public sealed class Branch
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = default!;
    public string Location { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected Branch() { }

    public Branch(string name, string location)
    {
        SetName(name);
        SetLocation(location);
    }

    public void Update(string name, string location, bool isActive)
    {
        SetName(name);
        SetLocation(location);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SalesDomainException("Nome da filial é obrigatório.");

        Name = name.Trim();
    }

    private void SetLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new SalesDomainException("Localização da filial é obrigatória.");

        Location = location.Trim();
    }
}
