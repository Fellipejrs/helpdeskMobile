using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace helpdesk
{
    public partial class AbrirChamadoPage : ContentPage
    {
        private readonly HttpClient _client;

        private async Task TestarToken()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            await DisplayAlert("TESTE TOKEN",
                $"Token no SecureStorage: {!string.IsNullOrEmpty(token)}\n" +
                $"Token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...",
                "OK");
        }

        public AbrirChamadoPage()
        {
            InitializeComponent();

            //_ = TestarToken();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri("http://localhost:5000");

            LoadToken();
        }

        private async void LoadToken()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async void OnEnviarChamadoClicked(object sender, EventArgs e)
        {
            string titulo = entryTitulo.Text;
            string descricao = editorDescricao.Text;
            string? prioridade = pickerPrioridade.SelectedItem?.ToString();
            string? categoria = pickerCategoria.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(descricao) ||
                prioridade == null ||
                categoria == null)
            {
                await DisplayAlert("Erro", "Preencha todos os campos antes de enviar.", "OK");
                return;
            }

            try
            {
                int userId = 1; // Temporário

                var novoChamado = new
                {
                    titulo = titulo,
                    descricao = descricao,
                    prioridade = prioridade,
                    categoria = categoria,
                    status = "Aberto",
                    usuario_id = userId,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                var response = await _client.PostAsJsonAsync("/api/Chamados", novoChamado);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", $"Chamado '{titulo}' criado com sucesso!", "OK");

                    entryTitulo.Text = string.Empty;
                    editorDescricao.Text = string.Empty;
                    pickerPrioridade.SelectedIndex = -1;
                    pickerCategoria.SelectedIndex = -1;

                    await Navigation.PopAsync();
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao criar chamado: {errorMsg}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro de conexão: {ex.Message}", "OK");
            }
        }

    }
}