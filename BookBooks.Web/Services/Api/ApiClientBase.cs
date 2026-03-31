using System.Net.Http.Json;

namespace BookBooks.Web.Services.Api;

public abstract class ApiClientBase
{
    protected static async Task<HttpResponseMessage> SendRequestAsync(
        Func<Task<HttpResponseMessage>> send,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await send();
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ApiException("A requisicao expirou ao conectar com a API.", 408);
        }
        catch (HttpRequestException)
        {
            throw new ApiException("Nao foi possivel conectar com a API. Verifique se o BookBooks.API esta em execucao.", 0);
        }
    }

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ApiErrorResponse? apiError = null;
        try
        {
            apiError = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        }
        catch
        {
        }

        var message = apiError?.Error;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = $"Request failed with status code {(int)response.StatusCode}.";
        }

        throw new ApiException(message, (int)response.StatusCode);
    }
}
