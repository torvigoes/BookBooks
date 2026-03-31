using System.Net.Http.Headers;

namespace BookBooks.Web.Services;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly AuthSession _authSession;

    public BearerTokenHandler(AuthSession authSession)
    {
        _authSession = authSession;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _authSession.CurrentUser?.Token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
