using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public UpdateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdTrackedAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

        if (await _repo.ExistsByDocumentAsync(request.Document, request.Id, ct))
            throw new SalesDomainException("Já existe outro cliente com este documento.");

        if (await _repo.ExistsByEmailAsync(request.Email, request.Id, ct))
            throw new SalesDomainException("Já existe outro cliente com este e-mail.");

        customer.Update(request.Name, request.Document, request.Email, request.Phone, request.IsActive);
        await _repo.UpdateAsync(customer, ct);

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
