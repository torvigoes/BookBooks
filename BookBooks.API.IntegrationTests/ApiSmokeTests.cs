using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookBooks.API.IntegrationTests;

public sealed class ApiSmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public ApiSmokeTests(TestWebApplicationFactory factory)
    {
        Environment.SetEnvironmentVariable("JwtOptions__SecretKey", "IntegrationTestSecretKeyWithEnoughLength123!");
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

    [Fact]
    public async Task GetReadingStatus_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/books/book-1/reading-status");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpsertReadingStatus_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var payload = new { status = 0 };

        var response = await client.PutAsJsonAsync("/api/books/book-1/reading-status", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLists_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/lists");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateList_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var payload = new { name = "My List", description = "desc", visibility = 0 };

        var response = await client.PostAsJsonAsync("/api/lists", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyFollowing_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/follows/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Follow_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsync("/api/follows/user-1", content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Unfollow_ShouldReturnUnauthorized_WhenNoBearerTokenIsProvided()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.DeleteAsync("/api/follows/user-1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
