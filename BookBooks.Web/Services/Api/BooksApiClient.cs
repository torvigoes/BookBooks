using System.Net.Http.Json;
using BookBooks.Web.Models.Books;

namespace BookBooks.Web.Services.Api;

public sealed class BooksApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public BooksApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/books/{id}", cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<BookDto>(cancellationToken))
               ?? throw new ApiException("Invalid book response payload.");
    }
}
