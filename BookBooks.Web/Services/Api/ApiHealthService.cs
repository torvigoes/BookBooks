namespace BookBooks.Web.Services.Api;

public sealed class ApiHealthService
{
    private readonly HttpClient _httpClient;

    public ApiHealthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsApiOnlineAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(
                "/api/books?page=1&pageSize=1",
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
