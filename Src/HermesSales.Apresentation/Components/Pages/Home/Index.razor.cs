using HermesSales.Apresentation.Handlers.Products.List;
using Microsoft.AspNetCore.Components;

namespace HermesSales.Apresentation.Components.Pages.Home;

public class IndexPageBase : ComponentBase
{
    [Inject] public ListProductsHandler ListProductsHandler { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    protected IEnumerable<ProductListItemModel> Products { get; set; } = new List<ProductListItemModel>();
    protected bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    protected async Task LoadProducts()
    {
        IsLoading = true;
        await Task.Delay(1000);
        try
        {
            Products = await ListProductsHandler.ExecuteAsync(new ListProductsQuery());
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void NavigateToProduct(Guid productId)
    {
        NavigationManager.NavigateTo($"/product/{productId}");
    }

    protected void AddToCart(Guid productId)
    {
        // Implementar lógica do carrinho depois
        Console.WriteLine($"Produto {productId} adicionado ao carrinho");
    }
}