using System.Linq;
using System.Windows.Controls;
using SmartGestionApp.Data;
using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Models;

namespace SmartGestionApp.Views.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly ClienteRepository _clienteRepo = new ClienteRepository(DatabaseManager.GetConnection().ConnectionString);
        private readonly TrabajoRepository _trabajoRepo = new TrabajoRepository(DatabaseManager.GetConnection().ConnectionString);
        private readonly PresupuestoRepository _presupuestoRepo = new PresupuestoRepository(DatabaseManager.GetConnection().ConnectionString);

        public DashboardPage()
        {
            InitializeComponent();
            CargarDashboard();
        }

        private void CargarDashboard()
        {
            // Totales
            var clientes = _clienteRepo.GetAll();
            var trabajos = _trabajoRepo.GetAll();
            var presupuestos = _presupuestoRepo.GetAll();

            TotalClientesText.Text = clientes.Count.ToString();
            TotalTrabajosText.Text = trabajos.Count.ToString();
            TotalPresupuestosText.Text = presupuestos.Count.ToString();

            // Cliente con más trabajos
            var clienteMasTrabajos = trabajos
                .GroupBy(t => t.ClienteId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    Cliente = clientes.FirstOrDefault(c => c.Id == g.Key),
                    Cantidad = g.Count()
                })
                .FirstOrDefault();

            if (clienteMasTrabajos is not null && clienteMasTrabajos.Cliente is not null)
            {
                TopClienteTrabajosText.Text = $"{clienteMasTrabajos.Cliente.Nombre} ({clienteMasTrabajos.Cantidad} trabajos)";
            }
            else
            {
                TopClienteTrabajosText.Text = "No hay datos.";
            }

            // Últimos trabajos
            var ultimos = trabajos
                .OrderByDescending(t => t.Fecha)
                .Take(5)
                .ToList();

            UltimosTrabajosList.ItemsSource = ultimos.Select(t =>
            {
                var cliente = clientes.FirstOrDefault(c => c.Id == t.ClienteId);
                return $"{t.Fecha.ToShortDateString()} - {cliente?.Nombre ?? "Cliente desconocido"} - {t.Descripcion}";
            });
        }
    }
}