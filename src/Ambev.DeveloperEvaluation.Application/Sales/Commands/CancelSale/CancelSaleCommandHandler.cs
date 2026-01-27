using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

public sealed class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponseDto>
{
    private readonly ISaleRepository _repo;
    private readonly ISaleAuditStore _audit;

    public CancelSaleCommandHandler(ISaleRepository repo, ISaleAuditStore audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<CancelSaleResponseDto> Handle(CancelSaleCommand request, CancellationToken ct)
    {
        var sale = await _repo.GetByIdAsync(request.SaleId, ct);
        if (sale is null)
            throw new SalesDomainException(SalesErrorMessages.SaleNotFound);

        // se quiser: impedir cancelamento duplicado aqui (caso seu domínio já não faça)
        sale.Cancel();

        await _repo.UpdateAsync(sale, ct);

        // auditoria mais útil (sem risco com Guid serializer, pois é string)
        await _audit.AppendAsync(
            eventType: "SaleCancelled",
            saleId: sale.Id,
            payload: new
            {
                SaleId = sale.Id.ToString(),
                Status = sale.Status.ToString(),
                TotalAmount = sale.TotalAmount,
                TotalDiscount = sale.TotalDiscount
            },
            ct);

        return new CancelSaleResponseDto
        {
            SaleId = sale.Id,
            Status = sale.Status.ToString(),
            CancelledAt = DateTime.UtcNow,
            TotalAmount = sale.TotalAmount,
            TotalDiscount = sale.TotalDiscount,
            Message = "Venda cancelada com sucesso."
        };
    }
}
