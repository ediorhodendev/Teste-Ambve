using Ambev.DeveloperEvaluation.IoC.Customers;
using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public sealed class CustomersModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddCustomersModule(builder.Configuration);
    }
}
