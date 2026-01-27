using Ambev.DeveloperEvaluation.IoC.Sales;
using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public sealed class SalesModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSalesModule(builder.Configuration);
    }
}
