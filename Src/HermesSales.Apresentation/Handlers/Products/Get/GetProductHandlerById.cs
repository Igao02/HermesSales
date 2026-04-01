using HermesSales.Application.UseCases.Products.GetProduct;

namespace HermesSales.Apresentation.Handlers.Products.Get;

public class GetProductHandlerById
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GetProductHandlerById(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GetProductResult> ExecuteAsync(GetProductModelInput model)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiBack");

            var response = await client.GetAsync($"/products/{model.Id}");

            if (response.IsSuccessStatusCode)
            {
                var productData = await response.Content.ReadFromJsonAsync<GetProductResponse>();

                if (productData is null)
                {
                    return new GetProductResult
                    {
                        Success = false,
                        Error = "Resposta da API veio vazia"
                    };
                }

                return new GetProductResult
                {
                    Success = true,
                    Data = productData
                };
            }
            else
            {
                return new GetProductResult
                {
                    Success = false,
                    Error = $"API error: {response.ReasonPhrase}"
                };
            }
        }
        catch (Exception ex)
        {
            return new GetProductResult
            {
                Success = false,
                Error = $"Exception: {ex.Message}"
            };
        }
    }

    public record GetProductModelInput(Guid Id);

    public record GetProductResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public GetProductResponse? Data { get; set; }
    }
}