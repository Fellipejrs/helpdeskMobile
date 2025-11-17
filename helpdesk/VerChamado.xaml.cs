using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace helpdesk
{
    public partial class VerChamado : ContentPage
    {
        private readonly HttpClient _client;
        public ObservableCollection<ChamadoViewModel> Chamados { get; set; } = new ObservableCollection<ChamadoViewModel>();

        public VerChamado()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri("http://localhost:5000");

            // Carregar token
            LoadToken();

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarChamadosAPI();
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

        private async void OnAbrirChamadoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AbrirChamadoPage());
        }

        private async Task CarregarChamadosAPI()
        {
            try
            {
                collectionChamados.IsVisible = false;
                Chamados.Clear();

                var response = await _client.GetAsync("/api/Chamados");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var chamadosAPI = System.Text.Json.JsonSerializer.Deserialize<List<ChamadoAPI>>(
                            jsonString,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                        if (chamadosAPI != null && chamadosAPI.Count > 0)
                        {

                            foreach (var chamado in chamadosAPI)
                            {
                                Chamados.Add(new ChamadoViewModel
                                {
                                    Id = chamado.id,
                                    Titulo = chamado.titulo ?? "Sem título",
                                    Descricao = chamado.descricao ?? "Sem descrição",
                                    Prioridade = chamado.prioridade ?? "Normal",
                                    Data = chamado.created_at?.ToString("dd/MM/yyyy HH:mm") ?? DateTime.Now.ToString("g"),
                                    Status = chamado.status ?? "Aberto"
                                });
                            }

                        }
                        else
                        {
                            await DisplayAlert("INFO", "Lista de chamados está vazia ou nula", "OK");
                            Chamados.Add(new ChamadoViewModel
                            {
                                Titulo = "Nenhum chamado encontrado",
                                Descricao = "Quando você criar chamados, eles aparecerão aqui.",
                                Prioridade = "Info",
                                Data = "",
                                Status = ""
                            });
                        }

                        collectionChamados.IsVisible = true;
                    }
                    catch (Exception jsonEx)
                    {
                        await DisplayAlert("ERRO JSON", $"Erro ao desserializar: {jsonEx.Message}", "OK");
                    }
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha na API: {errorMsg}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção: {ex.Message}", "OK");
            }

        }
        
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var item = e.CurrentSelection[0] as ChamadoViewModel;
                if (item != null)
                {
                    await DisplayAlert(
                        $"{item.Titulo} (ID: {item.Id})",
                        $"{item.Descricao}\n\n" +
                        $"Prioridade: {item.Prioridade}\n" +
                        $"Status: {item.Status}\n" +
                        $"Data: {item.Data}",
                        "Fechar");

                    // Deseleciona
                    ((CollectionView)sender).SelectedItem = null;
                }
            }
        }

        // 🔥 BOTÃO PARA RECARREGAR
        private async void OnRecarregarClicked(object sender, EventArgs e)
        {
            await CarregarChamadosAPI();
        }
    }

    // 🔥 CLASSE PARA RECEBER DADOS DA API
    public class ChamadoAPI
    {
        public int id { get; set; }
        public int? usuario_id { get; set; }
        public string? titulo { get; set; }
        public string? descricao { get; set; }
        public string? categoria { get; set; }
        public string? prioridade { get; set; }
        public string? status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    // 🔥 CLASSE PARA O FRONTEND (ViewModel)
    public class ChamadoViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}