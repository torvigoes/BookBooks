using BookBooks.Web.Models.Auth;

namespace BookBooks.Web.Services;

public sealed class AuthSession
{
    public event Action? OnChange;

    public AuthResponse? CurrentUser { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentUser?.Token);

    public void SignIn(AuthResponse user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
    }

    public void SignOut()
    {
        CurrentUser = null;
        OnChange?.Invoke();
    }
}
