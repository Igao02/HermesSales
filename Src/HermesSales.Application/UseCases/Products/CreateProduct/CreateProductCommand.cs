namespace HermesSales.Application.UseCases.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ApplicationUserId,
    string BaseUrl,
    IEnumerable<ProductImageInfo> Images);

public record ProductImageInfo(
    string FileName,
    byte[] Content,
    string ContentType);
