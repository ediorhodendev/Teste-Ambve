using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Common.Errors;

public sealed class ErrorResponse
{
    public string Code { get; init; } = "error";
    public string Message { get; init; } = "An error occurred.";
    public string TraceId { get; init; } = string.Empty;

    
    public IDictionary<string, string[]> Errors { get; init; }
        = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Details { get; init; } 
}
