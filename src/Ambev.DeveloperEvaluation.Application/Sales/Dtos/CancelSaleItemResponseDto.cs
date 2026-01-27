using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos
{
    public sealed class CancelSaleItemResponseDto
    {
        public Guid SaleId { get; init; }
        public Guid ItemId { get; init; }

        public string ItemStatus { get; init; } = default!;
        public DateTime CancelledAt { get; init; }

        public decimal ItemTotalAmount { get; init; }
        public decimal ItemDiscount { get; init; }

        public decimal SaleTotalAmount { get; init; }
        public decimal SaleTotalDiscount { get; init; }

        public string Message { get; init; } = default!;
    }

}
