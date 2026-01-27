using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;
