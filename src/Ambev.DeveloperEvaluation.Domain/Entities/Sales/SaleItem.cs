using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities.Sales;

public class SaleItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid SaleId { get; private set; }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal DiscountPercent { get; private set; } // 0..1 (ex.: 0.10 = 10%)
    public decimal DiscountValue { get; private set; }
    public decimal TotalAmount { get; private set; }

    public SaleItemStatus Status { get; private set; } = SaleItemStatus.Active;

    protected SaleItem() { }

    public SaleItem(
        Guid saleId,
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        SaleId = saleId;
        SetProduct(productId, productName);
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);

        ApplyDiscountRules();
        RecalculateTotals();
    }

    public void UpdateQuantity(int quantity)
    {
        EnsureActive();
        SetQuantity(quantity);
        ApplyDiscountRules();
        RecalculateTotals();
    }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        EnsureActive();
        SetUnitPrice(unitPrice);
        ApplyDiscountRules();
        RecalculateTotals();
    }

    public void Cancel()
    {
        if (Status == SaleItemStatus.Cancelled)
            throw new SalesDomainException(SalesErrorMessages.SaleItemAlreadyCancelled);

        Status = SaleItemStatus.Cancelled;
        DiscountPercent = 0;
        DiscountValue = 0;
        TotalAmount = 0;
    }

    private void SetProduct(Guid productId, string productName)
    {
        if (productId == Guid.Empty)
            throw new SalesDomainException("ProductId inválido.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new SalesDomainException("ProductName é obrigatório.");

        ProductId = productId;
        ProductName = productName.Trim();
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new SalesDomainException("Quantidade deve ser maior que zero.");

        Quantity = quantity;
    }

    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new SalesDomainException("Preço unitário deve ser maior que zero.");

        UnitPrice = unitPrice;
    }

    private void ApplyDiscountRules()
    {
        // Exemplo (ajuste às regras do desafio):
        // >= 4 e < 10 -> 10%
        // >= 10 e <= 20 -> 20%
        // > 20 -> inválido (ou bloqueia)
        if (Quantity > 20)
            throw new SalesDomainException("Quantidade máxima por item é 20.");

        DiscountPercent = Quantity switch
        {
            >= 4 and < 10 => 0.10m,
            >= 10 and <= 20 => 0.20m,
            _ => 0m
        };
    }

    private void RecalculateTotals()
    {
        if (Status == SaleItemStatus.Cancelled)
        {
            DiscountValue = 0;
            TotalAmount = 0;
            return;
        }

        var gross = Quantity * UnitPrice;
        DiscountValue = Math.Round(gross * DiscountPercent, 2);
        TotalAmount = Math.Round(gross - DiscountValue, 2);
    }

    private void EnsureActive()
    {
        if (Status == SaleItemStatus.Cancelled)
            throw new SalesDomainException(SalesErrorMessages.SaleItemAlreadyCancelled);
    }
}
