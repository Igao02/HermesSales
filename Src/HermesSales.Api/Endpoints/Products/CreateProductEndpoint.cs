using HermesSales.Api.Extensions;
using HermesSales.Api.Infrastructure;
using HermesSales.Application.UseCases.Products.CreateProduct;
using System.Security.Claims;

namespace HermesSales.Api.Endpoints.Products;

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/products/create", Handle)
            .RequireAuthorization()
            .DisableAntiforgery();
    }

    private static async Task<IResult> Handle(
        CreateProductRequest request,
        ClaimsPrincipal user,
        CreateProductUseCase useCase,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        if (request.Images?.Count() > 3)
        {
            return Results.BadRequest("O máximo permitido são 3 fotos.");
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var requestHost = context.Request;
        var baseUrl = $"{requestHost.Scheme}://{requestHost.Host}{requestHost.PathBase}";

        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            userId,
            baseUrl,
            request.Images?.Select(i => new ProductImageInfo(i.FileName, i.Content, i.ContentType)) ?? Enumerable.Empty<ProductImageInfo>());

        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.Match(
            response => Results.Created($"/products/create/{response.Id}", response),
            failure => CustomResults.Problem(failure));
    }

    private record CreateProductRequest(
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        IEnumerable<ImageRequest>? Images);

    private record ImageRequest(byte[] Content, string FileName, string ContentType);
}
