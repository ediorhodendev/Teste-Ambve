using Ambev.DeveloperEvaluation.Domain.Repositories.Products;
using Ambev.DeveloperEvaluation.ORM.Repositories.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Products;

public static class ProductsDependencyInjection
{
    public static IServiceCollection AddProductsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
