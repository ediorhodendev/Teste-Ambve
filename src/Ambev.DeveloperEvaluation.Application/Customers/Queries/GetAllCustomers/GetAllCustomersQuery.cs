using Ambev.DeveloperEvaluation.Application.Customers.Dtos;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery() : IRequest<List<CustomerDto>>;
