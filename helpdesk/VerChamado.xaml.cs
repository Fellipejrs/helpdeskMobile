using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace helpdesk
{
    public partial class VerChamado : ContentPage
    {
        public ObservableCollection<ChamadoViewModel> Chamados { get; set; } = new ObservableCollection<ChamadoViewModel>();

        public VerChamado()
        {
            InitializeComponent();

            // Mock data
            Chamados.Add(new ChamadoViewModel { Titulo = "Erro ao iniciar app", Descricao = "O aplicativo fecha ao iniciar.", Prioridade = "Alta", Data = DateTime.Now.ToString("g") });
            Chamados.Add(new ChamadoViewModel { Titulo = "Falha no login", Descricao = "Usuário não consegue logar com credenciais válidas.", Prioridade = "Média", Data = DateTime.Now.AddDays(-1).ToString("g") });
            Chamados.Add(new ChamadoViewModel { Titulo = "Sugestão de melhoria", Descricao = "Adicionar filtro por data.", Prioridade = "Baixa", Data = DateTime.Now.AddDays(-7).ToString("g") });

            collectionChamados.ItemsSource = Chamados;
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var item = e.CurrentSelection[0] as ChamadoViewModel;
                if (item != null)
                {
                    await DisplayAlert(item.Titulo, $"{item.Descricao}\n\nPrioridade: {item.Prioridade}\nData: {item.Data}", "Fechar");
                    // Deseleciona
                    ((CollectionView)sender).SelectedItem = null;
                }
            }
        }
    }

    public class ChamadoViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
