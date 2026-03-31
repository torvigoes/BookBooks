using System.Text.Json.Serialization;

namespace BookBooks.Web.Services.Api;

public sealed class ApiErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
