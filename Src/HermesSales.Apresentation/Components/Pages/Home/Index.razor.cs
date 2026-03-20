using HermesSales.Apresentation.Handlers.Products.List;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HermesSales.Apresentation.Components.Pages.Home;

public class IndexPageBase : ComponentBase
{
    [Inject] public ListProductsHandler ListProductsHandler { get; set; } = default!;

    protected IEnumerable<ProductListItemModel> Products { get; set; } = new List<ProductListItemModel>();
    protected bool IsLoading { get; set; } = true;

    // Dicionários para controle do carrossel (chave: Product Id do tipo Guid)
    private Dictionary<Guid, int> _currentImageIndex = new();
    private Dictionary<Guid, bool> _carouselVisibility = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    protected async Task LoadProducts()
    {
        IsLoading = true;
        await Task.Delay(1000); // Simula carregamento
        try
        {
            Products = await ListProductsHandler.ExecuteAsync(new ListProductsQuery());

            // Inicializa os índices do carrossel para cada produto
            foreach (var product in Products)
            {
                _currentImageIndex[product.Id] = 0;
                _carouselVisibility[product.Id] = false;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Métodos do Carrossel
    protected int GetCurrentImageIndex(Guid productId)
    {
        return _currentImageIndex.GetValueOrDefault(productId, 0);
    }

    protected bool IsCarouselVisible(Guid productId)
    {
        return _carouselVisibility.GetValueOrDefault(productId, false);
    }

    protected void ShowCarousel(Guid productId)
    {
        _carouselVisibility[productId] = true;
    }

    protected void HideCarousel(Guid productId)
    {
        _carouselVisibility[productId] = false;
    }

    protected void SetCurrentImage(Guid productId, int index)
    {
        if (_currentImageIndex.ContainsKey(productId))
        {
            _currentImageIndex[productId] = index;
        }
    }

    protected void NextImage(Guid productId, int totalImages)
    {
        if (_currentImageIndex.ContainsKey(productId))
        {
            _currentImageIndex[productId] = (_currentImageIndex[productId] + 1) % totalImages;
        }
    }

    protected void PreviousImage(Guid productId, int totalImages)
    {
        if (_currentImageIndex.ContainsKey(productId))
        {
            _currentImageIndex[productId] = (_currentImageIndex[productId] - 1 + totalImages) % totalImages;
        }
    }
}

