using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookBooks.Web;
using BookBooks.Web.Services;
using BookBooks.Web.Services.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var isHttpsHost = builder.HostEnvironment.BaseAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
var apiBaseUrl = isHttpsHost
    ? builder.Configuration["ApiBaseUrlHttps"] ?? "https://localhost:7007"
    : builder.Configuration["ApiBaseUrlHttp"] ?? "http://localhost:5247";

builder.Services.AddScoped<AuthSession>();
builder.Services.AddScoped<BearerTokenHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<BooksApiClient>();
builder.Services.AddScoped<ReviewsApiClient>();
builder.Services.AddScoped<ReadingStatusApiClient>();

var host = builder.Build();
await host.Services.GetRequiredService<AuthSession>().InitializeAsync();
await host.RunAsync();
