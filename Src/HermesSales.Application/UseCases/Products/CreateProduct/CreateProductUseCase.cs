using HermesSales.Application.Abstractions;
using HermesSales.Domain.Entities;
using HermesSales.SharedKernel;

namespace HermesSales.Application.UseCases.Products.CreateProduct;

public class CreateProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;

    public CreateProductUseCase(IProductRepository repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<Result<CreateProductResponse>> ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = new Product(
            command.Name,
            command.Description,
            command.Price,
            command.StockQuantity,
            DateTime.UtcNow,
            true,
            command.ApplicationUserId);

        foreach (var imageInfo in command.Images)
        {
            var relativePath = await _fileService.SaveFileAsync(imageInfo.Content, imageInfo.FileName, "images/products", cancellationToken);
            var fullUrl = $"{command.BaseUrl}{relativePath}";
            product.Images.Add(new ProductImage(product.Id, imageInfo.FileName, fullUrl));
        }

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var response = new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CreatedAt);

        return Result.Success(response);
    }
}
