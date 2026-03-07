using Microsoft.AspNetCore.Identity.Data;
using static HermesSales.Apresentation.Components.Pages.Account.User.Register.Index;

namespace HermesSales.Apresentation.Handlers;

public class AuthHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> Register(RegisterModel model)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");

        var request = new RegisterRequest
        {
            Email = model.Email,
            Password = model.Password
        };

        var response = await client.PostAsJsonAsync("/register", request);

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("STATUS: " + response.StatusCode);
        Console.WriteLine("BODY: " + content);

        return response.IsSuccessStatusCode;
    }
}
