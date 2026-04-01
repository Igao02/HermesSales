using HermesSales.Apresentation.Components;
using HermesSales.Apresentation.Extensions;
using HermesSales.Apresentation.Handlers;
using HermesSales.Apresentation.Handlers.Products.Create;
using HermesSales.Apresentation.Handlers.Products.Get;
using HermesSales.Apresentation.Handlers.Products.List;
using MudBlazor.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<CookieContainer>();
builder.Services.AddHttpClient("ApiBack", client =>
{
    client.BaseAddress = new Uri("https://localhost:7238");
})
.ConfigurePrimaryHttpMessageHandler(sp =>
{
    return new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = sp.GetRequiredService<CookieContainer>()
    };
});

// MudBlazor
builder.Services.AddMudServices();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Autenticação
builder.Services.AddAuthentication("Identity.Application")
    .AddCookie("Identity.Application");

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<ListProductsHandler>();
builder.Services.AddScoped<GetProductHandlerById>();

builder.Services.AddHttpContextAccessor();

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

app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAuthEndpoints();

app.Run();