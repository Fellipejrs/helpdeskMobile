using System.Net.Http.Json;
using System.Text.Json;

namespace helpdesk
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = entryUsuario.Text;
            string senha = entrySenha.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);
                client.BaseAddress = new Uri("http://localhost:5000");

                var response = await client.PostAsJsonAsync("/api/Auth/login", new { email, senha });

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    using var jsonDoc = JsonDocument.Parse(jsonString);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("token", out var tokenProperty))
                    {
                        var token = tokenProperty.GetString();

                        // 🔥 SALVAR O TOKEN no SecureStorage
                        await SecureStorage.SetAsync("jwt_token", token);

                        // Pegar outras informações
                        var name = root.GetProperty("name").GetString();
                        var role = root.GetProperty("role").GetString();

                        // Salvar outras informações se quiser
                        Preferences.Set("user_name", name);
                        Preferences.Set("user_role", role);

                        await DisplayAlert("Sucesso", $"Bem-vindo, {name}!\nToken salvo!", "OK");
                        await Navigation.PushAsync(new Home());
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Token não veio na resposta do login", "OK");
                    }
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha no login: {errorMsg}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro de conexão: {ex.Message}", "OK");
            }
        }
        public class LoginResponse
        {
            public string token { get; set; }
            public string name { get; set; }
            public string role { get; set; }
        }

        private async void OnCadastrarUsuarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cadastro());
        }
    }
}