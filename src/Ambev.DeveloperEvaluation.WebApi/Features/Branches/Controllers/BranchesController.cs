using Ambev.DeveloperEvaluation.Application.Branches.Commands.CreateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.Commands.DeleteBranch;
using Ambev.DeveloperEvaluation.Application.Branches.Commands.UpdateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.Queries.GetAllBranches;
using Ambev.DeveloperEvaluation.Application.Branches.Queries.GetBranchById;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.Controllers;

[ApiController]
[Route("api/branches")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class BranchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BranchesController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>Lista todas as filiais.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllBranchesQuery(), ct));

    /// <summary>Obtém uma filial pelo identificador.</summary>
    /// <param name="id">Id da filial.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetBranchByIdQuery(id), ct));

    /// <summary>Cria uma nova filial.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(CreateBranchRequestDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateBranchCommand(dto.Name, dto.Location),
            ct);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Atualiza uma filial.</summary>
    /// <param name="id">Id da filial.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Update(Guid id, UpdateBranchRequestDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(
            new UpdateBranchCommand(id, dto.Name, dto.Location, dto.IsActive),
            ct));

    /// <summary>Remove uma filial.</summary>
    /// <param name="id">Id da filial.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteBranchCommand(id), ct);
        return NoContent();
    }
}
