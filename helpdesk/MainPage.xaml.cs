using System.Net.Http.Json;

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

                client.BaseAddress = new Uri("https://localhost:7205");

                var loginData = new
                {
                    email = email,
                    senha = senha
                };

                var response = await client.PostAsJsonAsync("/api/Auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    await DisplayAlert("Sucesso", $"Bem-vindo, {content?.usuario.Name}!", "OK");

                    // aqui você pode salvar o token pra usar nas próximas requisições
                    string token = content.token;

                    // redireciona pra tela principal
                    await Navigation.PushAsync(new Home());
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

        private async void OnCadastrarUsuarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cadastro());
        }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public UsuarioResponse usuario { get; set; }
    }

    public class UsuarioResponse
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
