using BookBooks.Web.Models.Auth;
using System.Text.Json;
using Microsoft.JSInterop;

namespace BookBooks.Web.Services;

public sealed class AuthSession
{
    private const string StorageKey = "bookbooks.auth";
    private readonly IJSRuntime _jsRuntime;

    public event Action? OnChange;

    public AuthResponse? CurrentUser { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentUser?.Token);

    public AuthSession(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        var serialized = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return;
        }

        var user = JsonSerializer.Deserialize<AuthResponse>(serialized);
        if (user is null || string.IsNullOrWhiteSpace(user.Token))
        {
            return;
        }

        CurrentUser = user;
        OnChange?.Invoke();
    }

    public async Task SignInAsync(AuthResponse user)
    {
        CurrentUser = user;
        var serialized = JsonSerializer.Serialize(user);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, serialized);
        OnChange?.Invoke();
    }

    public async Task SignOutAsync()
    {
        CurrentUser = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        OnChange?.Invoke();
    }
}
