namespace HermesSales.Application.UseCases.Products.CreateProduct;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt);
