namespace BookBooks.Web.Services.Api;

public sealed class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}
