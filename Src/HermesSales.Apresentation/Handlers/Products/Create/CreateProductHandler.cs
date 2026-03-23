using System.Net.Http.Json;

namespace HermesSales.Apresentation.Handlers.Products.Create;

public class CreateProductHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateProductHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CreateProductResult> ExecuteAsync(CreateProductModel model)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");

        var response = await client.PostAsJsonAsync("/products/create", model);

        Console.WriteLine($"Resposta da API: StatusCode={response.StatusCode}, Content={await response.Content.ReadAsStringAsync()}");

        if (response.IsSuccessStatusCode)
            return new CreateProductResult { Success = true };

        return new CreateProductResult
        {
            Success = false,
            Error = "Erro ao cadastrar produto aqui no handler."
        };
    }
}

public record CreateProductModel(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IEnumerable<CreateProductImageModel>? Images = null);

public record CreateProductImageModel(byte[] Content, string FileName, string ContentType);

public class CreateProductResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}
