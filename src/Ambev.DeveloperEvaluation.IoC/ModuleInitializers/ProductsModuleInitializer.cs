using Ambev.DeveloperEvaluation.IoC.Products;
using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public sealed class ProductsModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddProductsModule(builder.Configuration);
    }
}
