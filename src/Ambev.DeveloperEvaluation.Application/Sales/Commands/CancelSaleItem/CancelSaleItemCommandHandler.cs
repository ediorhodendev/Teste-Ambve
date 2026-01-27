using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;

public sealed class CancelSaleItemCommandHandler
    : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _repo;
    private readonly ISaleAuditStore _audit;

    public CancelSaleItemCommandHandler(ISaleRepository repo, ISaleAuditStore audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetByIdAsync(request.SaleId, ct);
        if (sale is null)
            throw new SalesDomainException(SalesErrorMessages.SaleNotFound);

        var item = sale.CancelItem(request.ItemId);

        await _repo.UpdateAsync(sale, ct);

        await _audit.AppendAsync(
            "SaleItemCancelled",
            sale.Id,
            new
            {
                SaleId = sale.Id,
                ItemId = item.Id,
                ItemTotalAmount = item.TotalAmount,
                ItemDiscountValue = item.DiscountValue
            },
            ct);

        return new CancelSaleItemResult(
            sale.Id,
            item.Id,
            item.TotalAmount,
            item.DiscountValue,
            sale.TotalAmount,
            sale.TotalDiscount
        );
    }
}
