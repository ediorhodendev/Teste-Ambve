using System.Net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Npgsql;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var traceId = context.TraceIdentifier;

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. TraceId={TraceId} Path={Path}",
                traceId, context.Request.Path);

            var (status, response) = MapException(ex, traceId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private (HttpStatusCode Status, ErrorResponse Response) MapException(Exception ex, string traceId)
    {
        // 400 - FluentValidation
        if (ex is ValidationException vex)
        {
            var errors = vex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).Distinct().ToArray(),
                    StringComparer.OrdinalIgnoreCase);

            return (HttpStatusCode.BadRequest, new ErrorResponse
            {
                Code = "validation_error",
                Message = "Existem erros de validação na requisição.",
                TraceId = traceId,
                Errors = errors
            });
        }

        // 400 - Regras de domínio (ajuste se preferir 422)
        if (ex is SalesDomainException or DomainException)
        {
            return (HttpStatusCode.UnprocessableEntity, new ErrorResponse
            {
                Code = "domain_error",
                Message = ex.Message,
                TraceId = traceId
            });
        }

        // 409 - Chave única (Postgres)
        if (ex is DbUpdateException dbEx &&
            dbEx.InnerException is PostgresException pg &&
            pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // Ex: IX_sales_SaleNumber / ExternalId etc.
            var constraint = pg.ConstraintName ?? "unique_constraint";

            return (HttpStatusCode.Conflict, new ErrorResponse
            {
                Code = "conflict",
                Message = "Já existe um registro com os mesmos dados únicos.",
                TraceId = traceId,
                Errors = new Dictionary<string, string[]>
                {
                    ["constraint"] = new[] { constraint }
                }
            });
        }

        // 503 - Mongo indisponível / timeout (audit não gravou)
        if (ex is MongoException or TimeoutException)
        {
            return (HttpStatusCode.ServiceUnavailable, new ErrorResponse
            {
                Code = "dependency_unavailable",
                Message = "Dependência indisponível no momento (audit/log). Tente novamente.",
                TraceId = traceId
            });
        }

        
        return (HttpStatusCode.InternalServerError, new ErrorResponse
        {
            Code = "internal_error",
            Message = "Ocorreu um erro inesperado.",
            TraceId = traceId,
            Details = _env.IsDevelopment() ? new { ex.Message, ex.GetType().Name } : null
        });
    }
}
