using HermesSales.Domain.Entities;
using HermesSales.Domain.Enum;
using HermesSales.Domain.Repositories;
using HermesSales.SharedKernel;

namespace HermesSales.Application.UseCases.Products.CreateProduct;

public class CreateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IFileService _fileService;
    private readonly ILogProductRepository _logRepository;
    private readonly IProductImageRepository _productImageRepository;

    public CreateProductHandler(IProductRepository repository, IFileService fileService, ILogProductRepository logRep, IProductImageRepository imageRep)
    {
        _repository = repository;
        _fileService = fileService;
        _logRepository = logRep;
        _productImageRepository = imageRep;
    }

    public async Task<Result<CreateProductResponse>> ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Failure<CreateProductResponse>(new Error("500", "O nome do produto é obrigatório.", ErrorType.Problem));

        if (command.Name.Length > 200)
            return Result.Failure<CreateProductResponse>(new Error("500", "O nome do produto não pode ultrapassar 200 caracteres.", ErrorType.Problem));

        if (command.Price <= 0)
            return Result.Failure<CreateProductResponse>(new Error("500", "O preço deve ser maior que zero.", ErrorType.Problem));

        if (command.StockQuantity < 0)
            return Result.Failure<CreateProductResponse>(new Error("500", "A quantidade em estoque não pode ser negativa.", ErrorType.Problem));

        if (command.ApplicationUserId == Guid.Empty.ToString())
            return Result.Failure<CreateProductResponse>(new Error("500", "Usuário inválido.", ErrorType.Problem));

        if (command.Images != null)
        {
            foreach (var img in command.Images)
            {
                if (string.IsNullOrWhiteSpace(img.FileName))
                    return Result.Failure<CreateProductResponse>(new Error("500", "O nome de um dos arquivos de imagem está vazio.", ErrorType.Problem));

                if (img.Content is null || img.Content.Length == 0)
                    return Result.Failure<CreateProductResponse>(new Error("500", $"O conteúdo da imagem '{img.FileName}' está vazio.", ErrorType.Problem));
            }
        }

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
            await _repository.AddAsync(product, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            var log = new LogProduct(
                product.Id,
                null,
                product.Name,
                DateTime.UtcNow,
                true,
                string.Empty,
                command.ApplicationUserId,
                ProductLogAction.Created);

            await _logRepository.AddAsync(log, cancellationToken);
            await _logRepository.SaveAsync(cancellationToken);

            foreach (var imageInfo in command.Images ?? [])
            {
                var relativePath = await _fileService.SaveFileAsync(imageInfo.Content, imageInfo.FileName, "images/products", cancellationToken);
                var fullUrl = $"{command.BaseUrl}{relativePath}";

                var productImage = new ProductImage(product.Id, imageInfo.FileName, fullUrl, product.CreatedAt);

                await _productImageRepository.AddAsync(productImage, cancellationToken);
                await _productImageRepository.SaveAsync(cancellationToken);

                var logImage = new LogProduct(
                    product.Id,
                    productImage.Id,
                    $"Imagem: {imageInfo.FileName}",
                    DateTime.UtcNow,
                    true,
                    string.Empty,
                    command.ApplicationUserId,
                    ProductLogAction.CreatedImage);

                await _logRepository.AddAsync(logImage, cancellationToken);
                await _logRepository.SaveAsync(cancellationToken);
            }

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