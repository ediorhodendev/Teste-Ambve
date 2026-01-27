namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed class CancelSaleResponseDto
{
    public Guid SaleId { get; init; }
    public string Status { get; init; } = default!;
    public DateTime CancelledAt { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal TotalDiscount { get; init; }
    public string Message { get; init; } = default!;
}
