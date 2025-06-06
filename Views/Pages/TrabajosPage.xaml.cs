using SmartGestionApp.Data;
using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Models;
using SmartGestionApp.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SmartGestionApp.Views
{
    public partial class TrabajosPage : Page
    {
        private readonly TrabajoRepository _trabajoRepository;

        public TrabajosPage()
        {
            InitializeComponent();
            _trabajoRepository = new TrabajoRepository(DatabaseManager.GetConnection().ConnectionString);
            CargarTrabajos();
        }

        private void CargarTrabajos()
        {
            try
            {
                List<Trabajo> trabajos = _trabajoRepository.GetAll();
                TrabajosDataGrid.ItemsSource = trabajos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar trabajos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Trabajo? ObtenerTrabajoSeleccionado()
        {
            return TrabajosDataGrid.SelectedItem as Trabajo;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TrabajoDialog();
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                CargarTrabajos();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var trabajo = ObtenerTrabajoSeleccionado();
            if (trabajo == null)
            {
                MessageBox.Show("Por favor, seleccione un trabajo para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new TrabajoDialog(trabajo.Id, soloLectura: false);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                CargarTrabajos();
            }
        }

        private void BtnVer_Click(object sender, RoutedEventArgs e)
        {
            var trabajo = ObtenerTrabajoSeleccionado();
            if (trabajo == null)
            {
                MessageBox.Show("Por favor, seleccione un trabajo para ver.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new TrabajoDialog(trabajo.Id, soloLectura: true);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var trabajo = ObtenerTrabajoSeleccionado();
            if (trabajo == null)
            {
                MessageBox.Show("Por favor, seleccione un trabajo para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar el trabajo con ID {trabajo.Id}?",
                                            "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    _trabajoRepository.Delete(trabajo.Id);
                    MessageBox.Show("Trabajo eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarTrabajos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el trabajo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
