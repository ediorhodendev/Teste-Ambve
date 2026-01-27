using Ambev.DeveloperEvaluation.Domain.Repositories.Customers;
using Ambev.DeveloperEvaluation.ORM.Repositories.Customers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Customers;

public static class CustomersDependencyInjection
{
    public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }
}
