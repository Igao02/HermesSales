using HermesSales.Domain.Repositories;
using HermesSales.SharedKernel;

namespace HermesSales.Application.UseCases.Products.GetProduct;

public class GetProductHandler
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GetProductResponse>> ExecuteAsync(GetProductCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _repository.GetById(command.Id, cancellationToken);

            if (product is null)
            {
                var error = new Error("400", "Não foi possível encontrar o produto", ErrorType.Failure);
                return Result.Failure<GetProductResponse>(error);
            }

            var response = new GetProductResponse(
                  product.Name,
                  product.Description,
                  product.Price,
                  product.StockQuantity,
                  product.Images?.Select(i => i.FilePath).ToList() ?? new List<string>()
            );

            return Result.Success(response);
        }
        catch
        {
            return Result.Failure<GetProductResponse>(new Error("500", "Ocorreu um erro ao processar a solicitação", ErrorType.Problem));
        }
    }
}
