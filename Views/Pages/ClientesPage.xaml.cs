using SmartGestionApp.Data;
using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Models;
using SmartGestionApp.Views.Dialogs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SmartGestionApp.Views.Pages
{
    public partial class ClientesPage : UserControl
    {
        private ClienteRepository clienteRepo = new ClienteRepository(DatabaseManager.GetConnection().ConnectionString);
        private TrabajoRepository trabajoRepo = new TrabajoRepository(DatabaseManager.GetConnection().ConnectionString);

        public ClientesPage()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void CargarClientes()
        {
            var clientes = clienteRepo.GetAll();
            ClientesDataGrid.ItemsSource = clientes;
        }

        private void NuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ClienteDialog(); // deberías tener este dialog
            ventana.Owner = Window.GetWindow(this);
            if (ventana.ShowDialog() == true)
                CargarClientes();
        }

        private void VerCliente_Click(object sender, RoutedEventArgs e)
        {
            if (ObtenerClienteSeleccionado(sender) is Cliente cliente)
            {
                var dialog = new ClienteDialog(cliente, soloLectura: true);
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
            }
        }

        private void EditarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (ObtenerClienteSeleccionado(sender) is Cliente cliente)
            {
                var dialog = new ClienteDialog(cliente);
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == true)
                    CargarClientes();
            }
        }

        private void EliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (ObtenerClienteSeleccionado(sender) is not Cliente cliente) return;

            var trabajosAsociados = trabajoRepo.GetByClientId(cliente.Id);


            string mensaje = $"¿Estás seguro que deseas eliminar al cliente **{cliente.Nombre}**?";
            if (trabajosAsociados.Any())
            {
                mensaje += $"\n\nEste cliente tiene {trabajosAsociados.Count} trabajo(s) asociado(s):\n";
                mensaje += string.Join("\n", trabajosAsociados.Select(t => $"- {t.Descripcion}"));
                mensaje += "\n\n⚠️ Se eliminarán también esos trabajos.";
            }

            var resultado = MessageBox.Show(mensaje, "Confirmar eliminación",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                // Borramos trabajos primero
                foreach (var trabajo in trabajosAsociados)
                {
                    trabajoRepo.Delete(trabajo.Id);
                }

                // Luego el cliente
                clienteRepo.Delete(cliente.Id);
                MessageBox.Show("Cliente eliminado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CargarClientes();
            }
        }

        private Cliente ObtenerClienteSeleccionado(object sender)
        {
            var button = sender as Button;
            if (button?.DataContext is Cliente cliente)
                return cliente;

            return null;
        }
    }
}
