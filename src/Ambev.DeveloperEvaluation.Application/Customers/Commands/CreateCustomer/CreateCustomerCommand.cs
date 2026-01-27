using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Document,
    string Email,
    string Phone
) : IRequest<CustomerDto>;
