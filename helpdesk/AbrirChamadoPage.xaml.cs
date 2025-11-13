namespace helpdesk
{
    public partial class AbrirChamadoPage : ContentPage
    {
        public AbrirChamadoPage()
        {
            InitializeComponent();
        }

        private async void OnEnviarChamadoClicked(object sender, EventArgs e)
        {
            string titulo = entryTitulo.Text;
            string descricao = editorDescricao.Text;
            string? prioridade = pickerPrioridade.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descricao) || prioridade == null)
            {
                await DisplayAlert("Erro", "Preencha todos os campos antes de enviar.", "OK");
                return;
            }

            await DisplayAlert("Sucesso", $"Chamado '{titulo}' criado com prioridade {prioridade}.", "OK");

            entryTitulo.Text = string.Empty;
            editorDescricao.Text = string.Empty;
            pickerPrioridade.SelectedIndex = -1;
        }
    }
}
