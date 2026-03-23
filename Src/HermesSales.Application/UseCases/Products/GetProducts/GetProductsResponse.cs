namespace HermesSales.Application.UseCases.Products.GetProducts;

public record GetProductsResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IEnumerable<string> ImageUrls);
