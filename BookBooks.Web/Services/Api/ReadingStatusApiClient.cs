using System.Net.Http.Json;
using BookBooks.Web.Models.ReadingStatus;

namespace BookBooks.Web.Services.Api;

public sealed class ReadingStatusApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public ReadingStatusApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReadingStatusDto> GetByBookAsync(string bookId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/books/{bookId}/reading-status", cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReadingStatusDto>(cancellationToken))
               ?? throw new ApiException("Invalid reading status response payload.", 500);
    }

    public async Task<ReadingStatusDto> UpsertAsync(
        string bookId,
        UpdateReadingStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/books/{bookId}/reading-status", request, cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReadingStatusDto>(cancellationToken))
               ?? throw new ApiException("Invalid reading status response payload.", 500);
    }
}
