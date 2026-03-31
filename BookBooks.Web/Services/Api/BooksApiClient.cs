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

    public async Task<IReadOnlyCollection<BookDto>> SearchAsync(
        string searchTerm,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var encodedSearchTerm = Uri.EscapeDataString(searchTerm ?? string.Empty);
        var response = await _httpClient.GetAsync(
            $"/api/books?searchTerm={encodedSearchTerm}&page={page}&pageSize={pageSize}",
            cancellationToken);

        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookDto>>(cancellationToken))
               ?? [];
    }

    public async Task<string> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/books", request, cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<string>(cancellationToken))
               ?? throw new ApiException("Invalid create book response payload.");
    }
}
