using Ambev.DeveloperEvaluation.Application.Sales.Audit;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Audit
{
    public sealed class SaleAuditPayloadDto
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
        public List<SaleAuditItemPayloadDto> Items { get; set; } = new();
    }



}
