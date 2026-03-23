using System.Net.Http.Json;

namespace HermesSales.Apresentation.Handlers.Products.List;

public class ListProductsHandler
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ListProductsHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductListItemModel>> ExecuteAsync(ListProductsQuery query)
    {
        var client = _httpClientFactory.CreateClient("ApiBack");
        var response = await client.GetAsync("/products");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductListItemModel>>() 
                   ?? Enumerable.Empty<ProductListItemModel>();
        }

        return Enumerable.Empty<ProductListItemModel>();
    }
}

public record ListProductsQuery();

public record ProductListItemModel(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IEnumerable<string> ImageUrls);
