using System.Windows;

namespace PortalVideojuegos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string usuario) : this()
        {
            TxtBienvenida.Text = $"¡Bienvenido, {usuario}!";
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            // TC06: volver al login
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}