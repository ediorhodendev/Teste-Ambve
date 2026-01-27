using Ambev.DeveloperEvaluation.Domain.Repositories.Branches;
using Ambev.DeveloperEvaluation.ORM.Repositories.Branches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.Branches;

public static class BranchesDependencyInjection
{
    public static IServiceCollection AddBranchesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IBranchRepository, BranchRepository>();
        return services;
    }
}
