using System.Windows;
using SmartGestionApp.Properties;
using SmartGestionApp.Views.Pages;

namespace SmartGestionApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                MainFrame.Navigate(new DashboardPage()); // Página inicial
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar MainWindow: " + ex.Message);
            }
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void Clientes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientesPage());
        }

        private void Trabajos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TrabajosPage());
        }

        private void Presupuestos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PresupuestosPage());
        }

        private void Materiales_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MaterialesPage());
        }

        private void Graficos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GraficosPage());
        }

        private void Configuracion_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ConfiguracionPage());
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar credenciales almacenadas
            Settings.Default.Email = string.Empty;
            Settings.Default.PasswordHash = string.Empty;
            Settings.Default.Save();

            // Mostrar la ventana de inicio de sesión
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Cerrar la ventana principal
            this.Close();
        }

    }
}
