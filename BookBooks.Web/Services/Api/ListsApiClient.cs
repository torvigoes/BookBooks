using System.Net.Http.Json;
using BookBooks.Web.Models.Lists;

namespace BookBooks.Web.Services.Api;

public sealed class ListsApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public ListsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<BookListDto>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.GetAsync("/api/lists", cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BookListDto>>(cancellationToken))
               ?? [];
    }

    public async Task<BookListDto> GetByIdAsync(string listId, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.GetAsync($"/api/lists/{listId}", cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<BookListDto>(cancellationToken))
               ?? throw new ApiException("Invalid list response payload.", 500);
    }

    public async Task<BookListDto> CreateAsync(CreateListRequest request, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.PostAsJsonAsync("/api/lists", request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<BookListDto>(cancellationToken))
               ?? throw new ApiException("Invalid list response payload.", 500);
    }

    public async Task<BookListDto> AddBookAsync(
        string listId,
        AddBookToListRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.PostAsJsonAsync($"/api/lists/{listId}/books", request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<BookListDto>(cancellationToken))
               ?? throw new ApiException("Invalid list response payload.", 500);
    }

    public async Task<BookListDto> RemoveBookAsync(string listId, string bookId, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.DeleteAsync($"/api/lists/{listId}/books/{bookId}", cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<BookListDto>(cancellationToken))
               ?? throw new ApiException("Invalid list response payload.", 500);
    }

    public async Task DeleteAsync(string listId, CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.DeleteAsync($"/api/lists/{listId}", cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);
    }
}
