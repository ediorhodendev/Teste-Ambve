using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _repo.GetByIdAsync(request.Id, ct);
        if (customer is null)
            throw new SalesDomainException("Cliente não encontrado.");

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
