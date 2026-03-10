using HermesSales.Apresentation.Handlers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace HermesSales.Apresentation.Components.Pages.Account.User.Register;

public class IndexPageBase : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AuthHandler AuthHandler { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    public RegisterModel model { get; set; } = new();
    protected bool isLoading = false;

    public class RegisterModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Você deve aceitar os termos de uso")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve aceitar os termos de uso")]
        public bool AceitoTermos { get; set; }
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public string? errorMessage;

    protected async Task Register()
    {
        isLoading = true;
        try
        {
            if (!model.AceitoTermos)
            {
                Snackbar.Add("Você precisa aceitar os termos de uso para continuar.",
                    Severity.Warning);
                return;
            }

            var result = await AuthHandler.Register(model);

            // Feedback visual animado
            if (result.Success)
            {
                Snackbar.Add("Conta criada com sucesso! Verifique seu e-mail para ativação.",
                    Severity.Success,
                    config =>
                    {
                        config.ShowCloseIcon = true;
                        config.VisibleStateDuration = 5000;
                    });
                return;
                // Redirecionar para login ou dashboard
                // NavigationManager.NavigateTo("/login");
            }

            Snackbar.Add(result.Error ?? "Erro ao registrar usuário.", Severity.Error);

        }
        catch (Exception ex)
        {
            Snackbar.Add($"❌ Erro ao criar conta: {ex.Message}",
                Severity.Error,
                config =>
                {
                    config.ShowCloseIcon = true;
                    config.VisibleStateDuration = 5000;
                });
        }
        finally
        {
            isLoading = false;
        }
    }
}