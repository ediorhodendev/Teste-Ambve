using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        if (await _repo.ExistsByDocumentAsync(request.Document, null, ct))
            throw new SalesDomainException("Já existe cliente com este documento.");

        if (await _repo.ExistsByEmailAsync(request.Email, null, ct))
            throw new SalesDomainException("Já existe cliente com este e-mail.");

        var customer = new Customer(request.Name, request.Document, request.Email, request.Phone);
        await _repo.AddAsync(customer, ct);

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
