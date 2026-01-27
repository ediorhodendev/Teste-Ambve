using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _repo;

    public DeleteCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

        await _repo.DeleteAsync(customer, ct);
    }
}
