using HermesSales.Apresentation.Handlers;
using HermesSales.Apresentation.Handlers.Products.Create;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Security.Claims;

namespace HermesSales.Apresentation.Components.Pages.Product.Create;

public class IndexPageBase : ComponentBase
{
    protected readonly FormModel model = new();
    public IReadOnlyList<IBrowserFile> files = new List<IBrowserFile>();

    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] private CreateProductHandler CreateProductHandler { get; set; } = default!;

    protected bool isLoading = false;
    protected string selectedCategory = string.Empty;

    // Controle do Stepper
    protected bool Step1Completed => !string.IsNullOrWhiteSpace(model.Name);
    protected bool Step2Completed => Step1Completed && model.Price > 0;
    protected bool Step3Completed => Step2Completed && files.Any();

    protected bool IsFormValid =>
        !string.IsNullOrWhiteSpace(model.Name) &&
        model.Price > 0;

    protected class FormModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
    }

    protected void UploadFiles(IReadOnlyList<IBrowserFile> incomingFiles)
    {
        try
        {
            if (incomingFiles.Count > 3)
            {
                Snackbar.Add("Você pode selecionar no máximo 3 fotos.", Severity.Warning);
                return;
            }

            var invalidFiles = incomingFiles.Where(f => f.Size > 10 * 1024 * 1024).ToList();
            if (invalidFiles.Any())
            {
                Snackbar.Add("Alguns arquivos excedem o limite de 10MB.", Severity.Error);
                return;
            }

            files = incomingFiles;

            Snackbar.Add($"{files.Count} arquivo(s) selecionado(s) com sucesso!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao processar arquivos: {ex.Message}", Severity.Error);
        }
    }

    protected async Task Submit()
    {
        if (!IsFormValid)
        {
            Snackbar.Add("Por favor, preencha todos os campos obrigatórios.", Severity.Warning);
            return;
        }

        isLoading = true;

        try
        {
            var images = new List<CreateProductImageModel>();

            foreach (var file in files)
            {
                using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                var size = (int)file.Size;
                var buffer = new byte[size];

                int totalRead = 0;
                while (totalRead < size)
                {
                    int read = await stream.ReadAsync(buffer.AsMemory(totalRead, size - totalRead));
                    if (read == 0) break;
                    totalRead += read;
                }

                images.Add(new CreateProductImageModel(buffer, file.Name, file.ContentType));
            }

            var result = await CreateProductHandler.ExecuteAsync(new CreateProductModel(
                model.Name,
                model.Description,
                model.Price,
                model.StockQuantity,
                images));

            Console.WriteLine($"Resultado da criação: Success={result.Success}, Error={result.Error}");

            if (result.Success)
            {
                Snackbar.Add("✨ Produto cadastrado com sucesso!", Severity.Success, config =>
                {
                    config.ShowCloseIcon = true;
                    config.VisibleStateDuration = 4000;
                });

                await Task.Delay(500);
                ClearForm();
                Navigation.NavigateTo("/");
            }
            else
            {
                Console.WriteLine($"Erro ao cadastrar produto: {result.Error}");
                Snackbar.Add(result.Error ?? "Erro ao cadastrar produto no front.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"❌ Ocorreu um erro: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    protected void ClearForm()
    {
        model.Name = string.Empty;
        model.Description = string.Empty;
        model.Price = 0;
        model.StockQuantity = 0;
        files = new List<IBrowserFile>();
    }
}