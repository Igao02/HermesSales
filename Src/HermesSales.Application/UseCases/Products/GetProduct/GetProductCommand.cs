namespace HermesSales.Application.UseCases.Products.GetProduct;

public record GetProductCommand(
    Guid Id,
    string ApplicationUserId);
