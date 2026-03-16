using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace HermesSales.Apresentation.Handlers;

public class ProductHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductHandler(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ProductResult> CreateAsync(CreateProductModel model)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");

        var response = await client.PostAsJsonAsync("/products/create", model);

        Console.WriteLine($"Resposta da API: StatusCode={response.StatusCode}, Content={await response.Content.ReadAsStringAsync()}");

        if (response.IsSuccessStatusCode)
            return new ProductResult { Success = true };

        return new ProductResult
        {
            Success = false,
            Error = "Erro ao cadastrar produto aqui no handler."
        };
    }

    public record CreateProductModel(
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        IEnumerable<ImageModel>? Images = null);

    public record ImageModel(byte[] Content, string FileName, string ContentType);

    public class ProductResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}