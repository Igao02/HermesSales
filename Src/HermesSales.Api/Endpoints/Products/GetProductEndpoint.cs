using HermesSales.Api.Extensions;
using HermesSales.Application.UseCases.Products.GetProduct;
using System.Security.Claims;

namespace HermesSales.Api.Endpoints.Products;

public class GetProductEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id:guid}", Handle)
            .WithName("GetProduct")
            .WithTags("Products");
    }
    private static async Task<IResult> Handle(
        Guid id,
        GetProductHandler handle,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        if (string.IsNullOrEmpty(userId))
        {
            return Results.BadRequest("Usuário não encontrado!");
        }

        var command = new GetProductCommand(id, userId);

        var result = await handle.ExecuteAsync(command, cancellationToken);

        return result.Match(
            response => Results.Ok(response),
            failure => Results.BadRequest(failure));

    }

    private record GetProductRequest(
       Guid Id,
       string Description,
       decimal Price,
       int StockQuantity,
       IEnumerable<ImageRequest>? Images = null);

    private record ImageRequest(Guid Id, string FileName, string ContentType, byte[] Content);
}
