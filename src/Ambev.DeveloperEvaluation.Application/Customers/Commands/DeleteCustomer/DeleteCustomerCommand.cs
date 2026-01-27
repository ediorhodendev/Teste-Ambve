using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest;
