using System.Net.Http.Json;

namespace BookBooks.Web.Services.Api;

public abstract class ApiClientBase
{
    protected static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        var message = apiError?.Error;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = $"Request failed with status code {(int)response.StatusCode}.";
        }

        throw new ApiException(message);
    }
}
