using HermesSales.Application.UseCases.Products.GetProduct;
using HermesSales.Apresentation.Handlers.Products.Get;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Text;
using System.Text.Json;
using static HermesSales.Apresentation.Handlers.Products.Get.GetProductHandlerById;

public abstract class ProductDetailBase : ComponentBase
{
    [Parameter] public Guid Id { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected GetProductHandlerById GetProductHandlerById { get; set; } = default!;
    [Inject] protected IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] protected IConfiguration Configuration { get; set; } = default!;

    protected bool IsLoading { get; set; } = true;

    protected GetProductResponse? Product { get; set; }

    protected int quantity = 1;
    protected int currentImageIndex = 0;
    protected bool showLightbox = false;

    protected string userQuestion = string.Empty;
    protected bool isAiLoading = false;
    protected List<ChatMessage> chatMessages = new();

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

    protected List<string> quickSuggestions = new()
    {
        "Quais são as especificações técnicas?",
        "Este produto tem garantia?",
        "Para qual perfil de uso é recomendado?",
        "Quais são os diferenciais deste produto?"
    };

    protected async Task AskAI()
    {
        if (string.IsNullOrWhiteSpace(userQuestion) || Product == null) return;

        var question = userQuestion.Trim();
        userQuestion = string.Empty;

        chatMessages.Add(new ChatMessage(question, IsUser: true));
        isAiLoading = true;
        StateHasChanged();

        try
        {
            var answer = await CallGeminiAsync(question);
            chatMessages.Add(new ChatMessage(answer, IsUser: false));
        }
        catch (Exception ex)
        {
            chatMessages.Add(new ChatMessage(
                "Desculpe, não consegui processar sua pergunta no momento. Tente novamente.",
                IsUser: false));
            Snackbar.Add($"Erro ao consultar IA: {ex.Message}", Severity.Warning);
        }
        finally
        {
            isAiLoading = false;
            StateHasChanged();
        }
    }

    private async Task<string> SearchTavilyAsync(string productName)
    {
        var apiKey = Configuration["Tavily:ApiKey"];
        var httpClient = HttpClientFactory.CreateClient();

        var requestBody = new
        {
            api_key = apiKey,
            query = productName,
            search_depth = "basic",
            max_results = 3
        };

        var response = await httpClient.PostAsJsonAsync("https://api.tavily.com/search", requestBody);

        if (!response.IsSuccessStatusCode) return string.Empty;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Concatena os resultados encontrados
        var sb = new StringBuilder();
        foreach (var result in json.GetProperty("results").EnumerateArray())
        {
            var title = result.GetProperty("title").GetString();
            var content = result.GetProperty("content").GetString();
            sb.AppendLine($"- {title}: {content}");
        }

        return sb.ToString();
    }

    private async Task<string> CallGeminiAsync(string question)
    {
        var apiKey = Configuration["Gemini:ApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var productContext = $"""
        Nome: {Product!.Name}
        Descrição: {Product.Description}
        Preço: {Product.Price:C}
        Estoque disponível: {Product.StockQuantity} unidades
        """;

        // 👇 Busca em tempo real pelo nome do produto
        var searchResults = await SearchTavilyAsync(Product.Name);

        var historyContext = new StringBuilder();
        foreach (var msg in chatMessages.TakeLast(10))
            historyContext.AppendLine(msg.IsUser ? $"Cliente: {msg.Text}" : $"Assistente: {msg.Text}");

        var systemPrompt = $"""
        Você é um assistente de vendas amigável e prestativo.
        Responda perguntas sobre o produto abaixo seguindo esta ordem:
        1. Use PRIMEIRO as informações cadastradas do produto
        2. Se precisar de mais detalhes, use os resultados de busca fornecidos (que são sobre "{Product.Name}")
        3. NUNCA invente informações que não estejam nas fontes abaixo
        4. Se não souber, diga que não tem essa informação e sugira contato com o suporte
        Responda em português brasileiro, de forma concisa e objetiva.

        === INFORMAÇÕES DO PRODUTO ===
        {productContext}

        === RESULTADOS DE BUSCA EM TEMPO REAL ===
        {(string.IsNullOrEmpty(searchResults) ? "Nenhum resultado encontrado." : searchResults)}

        === HISTÓRICO DA CONVERSA ===
        {historyContext}
        Cliente: {question}
        Assistente:
        """;

        var requestBody = new
        {
            contents = new[]
            {
            new { parts = new[] { new { text = systemPrompt } } }
        },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 3500
            }
        };

        var httpClient = HttpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync(url, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "Não consegui gerar uma resposta.";
    }

    protected async Task AskSuggestion(string suggestion)
    {
        userQuestion = suggestion;
        await AskAI();
    }

    protected async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && (e.CtrlKey || e.ShiftKey))
        {
            await AskAI();
        }
    }

    protected record ChatMessage(string Text, bool IsUser);

    protected async Task AddToCart()
    {
        Snackbar.Add($"Produto adicionado ao carrinho! Quantidade: {quantity}", Severity.Success);
    }

    protected void SetCurrentImage(int index)
    {
        if (Product != null && Product.ImageUrls != null && index >= 0 && index < Product.ImageUrls.Count())
        {
            currentImageIndex = index;
            StateHasChanged();
        }
    }

    protected void PreviousImage()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            StateHasChanged();
        }
    }

    protected void NextImage()
    {
        if (Product != null && Product.ImageUrls != null && currentImageIndex < Product.ImageUrls.Count() - 1)
        {
            currentImageIndex++;
            StateHasChanged();
        }
    }

    protected void PreviousImageLightbox()
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            StateHasChanged();
        }
    }

    protected void NextImageLightbox()
    {
        if (Product != null && Product.ImageUrls != null && currentImageIndex < Product.ImageUrls.Count() - 1)
        {
            currentImageIndex++;
            StateHasChanged();
        }
    }

    protected void OpenLightbox()
    {
        showLightbox = true;
        StateHasChanged();
    }

    protected void CloseLightbox()
    {
        showLightbox = false;
        StateHasChanged();
    }
}