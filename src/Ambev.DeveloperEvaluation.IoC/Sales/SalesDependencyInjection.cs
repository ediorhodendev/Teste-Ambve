using Ambev.DeveloperEvaluation.Application.Sales.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.ORM.Mongo;
using Ambev.DeveloperEvaluation.ORM.Mongo.Stores;
using Ambev.DeveloperEvaluation.ORM.Repositories.Sales;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Sales;

public static class SalesDependencyInjection
{
    public static IServiceCollection AddSalesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Repository (Postgres)
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Mongo (Audit)
        services.Configure<MongoSettings>(
            configuration.GetSection("MongoSettings"));

        services.AddSingleton<MongoContext>();
        services.AddScoped<ISaleAuditStore, SaleAuditStore>();

        return services;
    }
}
