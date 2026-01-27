using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string Document,
    string Email,
    string Phone,
    bool IsActive
) : IRequest<CustomerDto>;
