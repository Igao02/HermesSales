using HermesSales.Apresentation.Components;
using HermesSales.Apresentation.Handlers;
using Microsoft.AspNetCore.Authentication;
using MudBlazor.Services;
using System.Net;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ApiBack", client =>
{
    client.BaseAddress = new Uri("https://localhost:7238");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new CookieContainer()
    };
});

// MudBlazor
builder.Services.AddMudServices();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Autenticação (somente cookies)
builder.Services.AddAuthentication("Identity.Application")
    .AddCookie("Identity.Application");

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthHandler>();

builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/login", async (HttpContext context, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("ApiBack");
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

    var response = await client.PostAsync(
        "/login?useCookies=true",
        new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

    if (response.IsSuccessStatusCode)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "usuario")
        };
        var identity = new ClaimsIdentity(claims, "Identity.Application");
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync("Identity.Application", principal);
        context.Response.StatusCode = 200;
    }
    else
    {
        context.Response.StatusCode = (int)response.StatusCode;
        await response.Content.CopyToAsync(context.Response.Body);
    }
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync("Identity.Application");
    context.Response.StatusCode = 200;
}).DisableAntiforgery();

app.MapGet("/auth/test", (ClaimsPrincipal user) => user.Identity?.IsAuthenticated == true ? $"Logado como {user.Identity.Name}" : "Não autenticado");

app.Run();