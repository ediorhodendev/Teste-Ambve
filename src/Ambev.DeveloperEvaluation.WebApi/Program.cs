using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        // ✅ Bootstrap logger SEMPRE no console (antes de tudo)
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);



            // ✅ Garante Serilog no console também após carregar config
            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration)
                  .Enrich.FromLogContext()
                  .WriteTo.Console();
            });

            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );


           

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

            builder.Services.AddTransient<ExceptionHandlingMiddleware>();
            
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();




            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                Log.Information("Applying EF Core migrations...");
                db.Database.Migrate();
                Log.Information("Migrations applied successfully.");
            }
            catch (Exception ex)
            {
                // 👇 isso garante que aparece em docker logs MESMO se serilog falhar
                Console.Error.WriteLine(ex.ToString());
                Log.Fatal(ex, "Migration failed. Application will stop.");
                throw;
            }

            app.UseMiddleware<ValidationExceptionMiddleware>();

            // ✅ Swagger sempre (pra avaliador não depender de env)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Developer Evaluation API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            // 👇 garante log no docker logs
            Console.Error.WriteLine(ex.ToString());
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
