using Ambev.DeveloperEvaluation.Application.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/sales")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>Cria uma venda com itens.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SaleDto>> Create(CreateSaleRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSaleCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Obtém uma venda pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSaleByIdQuery(id), ct));

    /// <summary>Lista vendas com filtros e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<SaleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<SaleDto>>> Get(
        [FromQuery] SalesFilterDto filter,
        CancellationToken ct)
        => Ok(await _mediator.Send(new GetSalesQuery(filter), ct));

    /// <summary>Atualiza uma venda.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SaleDto>> Update(Guid id, UpdateSaleRequestDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateSaleCommand(id, dto), ct));

    /// <summary>Cancela uma venda.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(CancelSaleResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CancelSaleResponseDto>> Cancel(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new CancelSaleCommand(id), ct));

    /// <summary>Cancela um item de uma venda.</summary>
    [HttpPost("{saleId:guid}/items/{itemId:guid}/cancel")]
    [ProducesResponseType(typeof(CancelSaleItemResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CancelSaleItemResponseDto>> CancelItem(
        Guid saleId,
        Guid itemId,
        CancellationToken ct)
        => Ok(await _mediator.Send(new CancelSaleItemCommand(saleId, itemId), ct));
}
