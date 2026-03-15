using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace HermesSales.Apresentation.Extensions;

public static class AuthEndpointsExtension
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
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
    }
}
