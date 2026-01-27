using Ambev.DeveloperEvaluation.Application.Products.Commands.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.Commands.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.Commands.UpdateProduct;
using Ambev.DeveloperEvaluation.Application.Products.Queries.GetAllProducts;
using Ambev.DeveloperEvaluation.Application.Products.Queries.GetProductById;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>Lista todos os produtos.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllProductsQuery(), ct));

    /// <summary>Obtém um produto pelo Id.</summary>
    /// <param name="id">Id do produto.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetProductByIdQuery(id), ct));

    /// <summary>Cria um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(CreateProductRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateProductCommand(dto.ExternalId, dto.Name, dto.Description, dto.Price),
            ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Atualiza um produto.</summary>
    /// <param name="id">Id do produto.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Update(Guid id, UpdateProductRequestDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateProductCommand(id, dto.ExternalId, dto.Name, dto.Description, dto.Price, dto.IsActive),
            ct));

    /// <summary>Remove um produto.</summary>
    /// <param name="id">Id do produto.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}
