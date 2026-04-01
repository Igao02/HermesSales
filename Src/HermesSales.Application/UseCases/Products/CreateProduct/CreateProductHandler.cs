using HermesSales.Domain.Entities;
using HermesSales.Domain.Repositories;
using HermesSales.SharedKernel;

namespace HermesSales.Application.UseCases.Products.CreateProduct;

public class CreateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;

    public CreateProductHandler(IProductRepository repository, IFileService fileService)
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

        try 
        {
            foreach (var imageInfo in command.Images)
            {
                var relativePath = await _fileService.SaveFileAsync(imageInfo.Content, imageInfo.FileName, "images/products", cancellationToken);
                var fullUrl = $"{command.BaseUrl}{relativePath}";
                product.Images.Add(new ProductImage(product.Id, imageInfo.FileName, fullUrl));
            }

            await _repository.AddAsync(product, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            
            Console.WriteLine("[USECASE] Produto salvo com sucesso no banco.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[USECASE] ERRO CRÍTICO AO SALVAR: {ex.Message}");
            if (ex.InnerException != null) 
                Console.WriteLine($"[USECASE] DETALHE DO ERRO: {ex.InnerException.Message}");
            throw;
        }

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
