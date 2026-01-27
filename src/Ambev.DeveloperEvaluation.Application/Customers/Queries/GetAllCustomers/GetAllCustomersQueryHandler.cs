using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;

public sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repo;

    public GetAllCustomersQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        var list = await _repo.GetAllAsync(ct);

        return list.Select(customer => new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        }).ToList();
    }
}
