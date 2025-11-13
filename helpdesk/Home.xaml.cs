namespace helpdesk
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert("Sair", "Deseja realmente sair?", "Sim", "Não");

            if (confirmar)
            {
                await Navigation.PopToRootAsync(); // Volta para a tela de login
            }
        }

        private async void OnAbrirChamadoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AbrirChamadoPage());
        }

        private async void OnVerChamadosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VerChamado());
        }
    }
}
