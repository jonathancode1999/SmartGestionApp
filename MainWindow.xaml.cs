using System.Windows;
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
                MainFrame.Navigate(new ClientesPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar MainWindow: " + ex.Message);
            }
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
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
