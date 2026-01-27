using Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;
using Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;
using Ambev.DeveloperEvaluation.WebApi.Features.Customers.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/customers")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>Lista todos os clientes.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllCustomersQuery(), ct));

    /// <summary>Obtém um cliente pelo Id.</summary>
    /// <param name="id">Id do cliente.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerByIdQuery(id), ct));

    /// <summary>Cria um novo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(CreateCustomerRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateCustomerCommand(dto.Name, dto.Document, dto.Email, dto.Phone),
            ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Atualiza um cliente.</summary>
    /// <param name="id">Id do cliente.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Update(Guid id, UpdateCustomerRequestDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateCustomerCommand(id, dto.Name, dto.Document, dto.Email, dto.Phone, dto.IsActive),
            ct));

    /// <summary>Remove um cliente.</summary>
    /// <param name="id">Id do cliente.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCustomerCommand(id), ct);
        return NoContent();
    }
}
