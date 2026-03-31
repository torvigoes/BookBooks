using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookBooks.API.IntegrationTests;

public sealed class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateReview_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var payload = new
        {
            rating = 5,
            content = "This review content is valid and long enough.",
            containsSpoiler = false
        };

        var response = await client.PostAsJsonAsync("/api/books/book-1/reviews", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var payload = new
        {
            title = "",
            author = "",
            isbn = "123",
            year = 0,
            coverImageUrl = (string?)null
        };

        var response = await client.PostAsJsonAsync("/api/books", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
