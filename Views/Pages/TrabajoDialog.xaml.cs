using SmartGestionApp.Data;
using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Models;
using System;
using System.Windows;

namespace SmartGestionApp.Views
{
    public partial class TrabajoDialog : Window
    {
        private readonly TrabajoRepository _trabajoRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly UsuarioRepository _usuarioRepo;
        private readonly EstadoTrabajoRepository _estadoRepo;
        private readonly TipoTrabajoRepository _tipoTrabajoRepo;

        public Trabajo? Trabajo { get; private set; }

        private bool isEditMode = false;
        private bool _soloLectura = false;

        // Constructor para nuevo trabajo o edición
        public TrabajoDialog(Trabajo? trabajo = null)
        {
            InitializeComponent();

            string connStr = DatabaseManager.GetConnection().ConnectionString;
            _trabajoRepo = new TrabajoRepository(connStr);
            _clienteRepo = new ClienteRepository(connStr);
            _usuarioRepo = new UsuarioRepository(connStr);
            _estadoRepo = new EstadoTrabajoRepository(connStr);
            _tipoTrabajoRepo = new TipoTrabajoRepository(connStr);

            LoadCombos();

            if (trabajo != null)
            {
                isEditMode = true;
                Trabajo = trabajo;
                MapToForm(trabajo);
            }
            else
            {
                dpFecha.SelectedDate = DateTime.Today;
            }
        }

        // Constructor para modo lectura desde el ID
        public TrabajoDialog(int trabajoId, bool soloLectura = false)
        {
            InitializeComponent();

            string connStr = DatabaseManager.GetConnection().ConnectionString;
            _trabajoRepo = new TrabajoRepository(connStr);
            _clienteRepo = new ClienteRepository(connStr);
            _usuarioRepo = new UsuarioRepository(connStr);
            _estadoRepo = new EstadoTrabajoRepository(connStr);
            _tipoTrabajoRepo = new TipoTrabajoRepository(connStr);

            LoadCombos();

            _soloLectura = soloLectura;
            Trabajo = _trabajoRepo.GetById(trabajoId);

            if (Trabajo != null)
            {
                isEditMode = true;
                MapToForm(Trabajo);
            }
            else
            {
                MessageBox.Show("No se encontró el trabajo especificado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (_soloLectura)
            {
                SetSoloLectura();
            }
        }

        private void LoadCombos()
        {
            try
            {
                var clientes = _clienteRepo.GetAll();
                var usuarios = _usuarioRepo.GetAll();
                var estados = _estadoRepo.GetAll();
                var tipos = _tipoTrabajoRepo.GetAll();

                estados.Insert(0, new EstadoTrabajo { Id = 0, Nombre = "(Sin estado)" });
                tipos.Insert(0, new TipoTrabajo { Id = 0, Nombre = "(Sin tipo)" });

                cbClientes.ItemsSource = clientes;
                cbUsuarios.ItemsSource = usuarios;
                cbEstados.ItemsSource = estados;
                cbTiposTrabajo.ItemsSource = tipos;

                cbEstados.SelectedIndex = 0;
                cbTiposTrabajo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MapToForm(Trabajo trabajo)
        {
            cbClientes.SelectedValue = trabajo.ClienteId;
            cbUsuarios.SelectedValue = trabajo.UsuarioId;

            cbEstados.SelectedValue = trabajo.EstadoId.HasValue && trabajo.EstadoId.Value != 0
                ? trabajo.EstadoId.Value
                : 0;

            cbTiposTrabajo.SelectedValue = trabajo.TipoTrabajoId.HasValue && trabajo.TipoTrabajoId.Value != 0
                ? trabajo.TipoTrabajoId.Value
                : 0;

            txtDescripcion.Text = trabajo.Descripcion ?? "";
            dpFecha.SelectedDate = trabajo.Fecha;
        }

        private void SetSoloLectura()
        {
            dpFecha.IsEnabled = false;
            cbClientes.IsEnabled = false;
            cbUsuarios.IsEnabled = false;
            cbEstados.IsEnabled = false;
            cbTiposTrabajo.IsEnabled = false;
            txtDescripcion.IsReadOnly = true;
            btnGuardar.Visibility = Visibility.Collapsed;
        }

        private bool ValidateForm()
        {
            if (cbClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un Cliente.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un Usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una Fecha válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (_soloLectura) return;
            if (!ValidateForm()) return;

            try
            {
                if (!isEditMode)
                    Trabajo = new Trabajo();

                Trabajo.ClienteId = (int)cbClientes.SelectedValue;
                Trabajo.UsuarioId = (int)cbUsuarios.SelectedValue;

                var estado = cbEstados.SelectedItem as EstadoTrabajo;
                Trabajo.EstadoId = (estado != null && estado.Id != 0) ? estado.Id : null;

                var tipo = cbTiposTrabajo.SelectedItem as TipoTrabajo;
                Trabajo.TipoTrabajoId = (tipo != null && tipo.Id != 0) ? tipo.Id : null;

                Trabajo.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim();
                Trabajo.Fecha = dpFecha.SelectedDate.Value;

                if (isEditMode)
                {
                    _trabajoRepo.Update(Trabajo);
                }
                else
                {
                    Trabajo.CreatedAt = DateTime.Now;
                    _trabajoRepo.Insert(Trabajo);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando trabajo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
