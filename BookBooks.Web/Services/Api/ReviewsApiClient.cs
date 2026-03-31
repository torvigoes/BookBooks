using System.Net.Http.Json;
using BookBooks.Web.Models.Reviews;

namespace BookBooks.Web.Services.Api;

public sealed class ReviewsApiClient : ApiClientBase
{
    private readonly HttpClient _httpClient;

    public ReviewsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetByBookAsync(
        string bookId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.GetAsync(
                $"/api/books/{bookId}/reviews?page={page}&pageSize={pageSize}",
                cancellationToken),
            cancellationToken);

        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ReviewDto>>(cancellationToken))
               ?? [];
    }

    public async Task CreateAsync(
        string bookId,
        CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(
            () => _httpClient.PostAsJsonAsync($"/api/books/{bookId}/reviews", request, cancellationToken),
            cancellationToken);
        await EnsureSuccessAsync(response);
    }
}
