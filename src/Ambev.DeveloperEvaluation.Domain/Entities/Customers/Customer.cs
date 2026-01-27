using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Customers;

public sealed class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = default!;
    public string Document { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    protected Customer() { }

    public Customer(string name, string document, string email, string phone)
    {
        SetName(name);
        SetDocument(document);
        SetEmail(email);
        SetPhone(phone);
    }

    public void Update(string name, string document, string email, string phone, bool isActive)
    {
        SetName(name);
        SetDocument(document);
        SetEmail(email);
        SetPhone(phone);
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new SalesDomainException("Nome do cliente é obrigatório.");

        Name = name.Trim();
    }

    private void SetDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new SalesDomainException("Documento do cliente é obrigatório.");

        Document = document.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new SalesDomainException("E-mail do cliente é obrigatório.");

        Email = email.Trim();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new SalesDomainException("Telefone do cliente é obrigatório.");

        Phone = phone.Trim();
    }
}
