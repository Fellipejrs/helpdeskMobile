using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace helpdesk
{
    public partial class Cadastro : ContentPage
    {
        private readonly HttpClient _client;

        public Cadastro()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _client = new HttpClient(handler);

            _client.BaseAddress = new Uri("http://localhost:5000");
        }

        public async void OnCadastrarClicked(object sender, EventArgs e)
        {
            string nome = entryNome.Text;
            string email = entryEmail.Text;
            string senha = entrySenha.Text;
            string tipoSelecionado = pickerTipo.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha) ||
                string.IsNullOrWhiteSpace(tipoSelecionado))
            {
                await DisplayAlert("Erro", "Preencha todos os campos antes de cadastrar.", "OK");
                return;
            }

            string role = tipoSelecionado.ToLower() switch
            {
                "administrador" => "tecnico",
                "usuario" => "usuario",
                _ => "usuario"
            };

            var usuario = new
            {
                name = nome,
                email = email,
                senha = senha,
                role = role
            };

            try
            {
                var response = await _client.PostAsJsonAsync("/api/Auth/registrar", usuario);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Usuário cadastrado com sucesso!", "OK");

                    entryNome.Text = string.Empty;
                    entryEmail.Text = string.Empty;
                    entrySenha.Text = string.Empty;
                    pickerTipo.SelectedIndex = -1;
                    switchAtivo.IsToggled = true;

                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao cadastrar: {errorMsg}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro de conexão: {ex.Message}", "OK");
            }
        }
    }
}