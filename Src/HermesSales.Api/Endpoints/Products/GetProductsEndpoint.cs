using HermesSales.Api.Extensions;
using HermesSales.Api.Infrastructure;
using HermesSales.Application.UseCases.Products.GetProducts;

namespace HermesSales.Api.Endpoints.Products;

public class GetProductsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", Handle)
            .AllowAnonymous()
            .WithName("GetProducts")
            .WithTags("Products");
    }

    private static async Task<IResult> Handle(
        GetProductsHandler useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetProductsCommand(), cancellationToken);

        return result.Match(
            response => Results.Ok(response),
            failure => CustomResults.Problem(failure));
    }
}
