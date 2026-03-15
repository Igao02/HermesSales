namespace HermesSales.Apresentation.Handlers;

public class ProductHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ProductResult> CreateAsync(CreateProductModel model)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");

        var response = await client.PostAsJsonAsync("/products/create", model);

        if (response.IsSuccessStatusCode)
            return new ProductResult { Success = true };

        return new ProductResult { Success = false, Error = "Erro ao cadastrar produto." };
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
