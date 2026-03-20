using HermesSales.Domain.Repositories;
using HermesSales.SharedKernel;

namespace HermesSales.Application.UseCases.Products.GetProducts;

public class GetProductsHandler
{
    private readonly IProductRepository _repository;

    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<GetProductsResponse>>> ExecuteAsync(GetProductsCommand command, CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllWithImagesAsync(cancellationToken);

        var response = products.Select(p => new GetProductsResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.Images.Select(i => i.FilePath)));

        return Result.Success(response);
    }
}
