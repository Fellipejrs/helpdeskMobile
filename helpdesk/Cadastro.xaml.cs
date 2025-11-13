using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace helpdesk
{
    public partial class Cadastro : ContentPage
    {
        public Cadastro()
        {
            InitializeComponent();
        }
        public async void OnCadastrarClicked(object sender, EventArgs e)
        {
            string nome = entryNome.Text;
            string email = entryEmail.Text;
            string senha = entrySenha.Text;
            string tipo = pickerTipo.SelectedItem?.ToString() ?? string.Empty;
            bool ativo = switchAtivo.IsToggled;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha) ||
                string.IsNullOrWhiteSpace(tipo))
            {
                await DisplayAlert("Erro", "Preencha todos os campos antes de cadastrar.", "OK");
                return;
            }

            var usuario = new
            {
                nome,
                email,
                senha,
                tipo,
                ativo
            };

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://10.0.2.2:7205");

                var response = await client.PostAsJsonAsync("/api/Auth/registrar", usuario);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Usuário cadastrado com sucesso!", "OK");
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

            entryNome.Text = string.Empty;
            entryEmail.Text = string.Empty;
            entrySenha.Text = string.Empty;
            pickerTipo.SelectedIndex = -1;
            switchAtivo.IsToggled = true;
        }

    }
}