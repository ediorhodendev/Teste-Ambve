using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos;

public sealed class SaleDto
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = default!;
    public DateTime SaleDate { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = default!;

    public SaleStatus Status { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }

    public List<SaleItemDto> Items { get; set; } = new();
}

