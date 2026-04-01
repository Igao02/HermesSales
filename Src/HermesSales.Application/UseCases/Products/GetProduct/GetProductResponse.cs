namespace HermesSales.Application.UseCases.Products.GetProduct;

public record GetProductResponse(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IEnumerable<string> ImageUrls);
