using Ambev.DeveloperEvaluation.IoC.Branches;
using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public sealed class BranchesModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddBranchesModule(builder.Configuration);
    }
}
