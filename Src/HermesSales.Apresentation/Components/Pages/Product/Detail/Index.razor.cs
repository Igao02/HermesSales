using HermesSales.Application.UseCases.Products.GetProduct;
using HermesSales.Apresentation.Handlers.Products.Get;
using Microsoft.AspNetCore.Components;
using MudBlazor;

using static HermesSales.Apresentation.Handlers.Products.Get.GetProductHandlerById;

public abstract class ProductDetailBase : ComponentBase
{
    [Parameter] public Guid Id { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected GetProductHandlerById GetProductHandlerById { get; set; } = default!;

    protected bool IsLoading { get; set; } = true;

    protected GetProductResponse? Product { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProduct();
    }

    protected async Task LoadProduct()
    {
        IsLoading = true;

        try
        {
            var result = await GetProductHandlerById.ExecuteAsync(new GetProductModelInput(Id));

            if (result.Success)
            {
                Product = result.Data;
            }
            else
            {
                Snackbar!.Add(result.Error ?? "Erro ao carregar produto", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar produto: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
}