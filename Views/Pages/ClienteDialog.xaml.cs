using SmartGestionApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CountryData;
using System.Collections.Generic;
using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Data;




namespace SmartGestionApp.Views.Dialogs
{
    public partial class ClienteDialog : Window
    {
        public Cliente Cliente { get; private set; }
        private readonly bool soloLectura;

        private ObservableCollection<ClienteTelefono> Telefonos;
        private ObservableCollection<ClienteEmail> Emails;
        private readonly ClienteRepository _clienteRepo;



        public ClienteDialog(Cliente cliente = null, bool soloLectura = false)
        {
            var allCountryInfo = CountryLoader.CountryInfo;

            InitializeComponent();
            _clienteRepo = new ClienteRepository(DatabaseManager.GetConnection().ConnectionString);
            cmbProvincia.SelectionChanged += cmbProvincia_SelectionChanged;

            this.soloLectura = soloLectura;

            // Cargar países
            cmbPais.ItemsSource = CountryLoader.CountryInfo
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToList();


            if (cliente != null)
            {
                Cliente = cliente;
                txtNombre.Text = cliente.Nombre;

                Telefonos = new ObservableCollection<ClienteTelefono>(cliente.Telefonos ?? new System.Collections.Generic.List<ClienteTelefono>());
                Emails = new ObservableCollection<ClienteEmail>(cliente.Emails ?? new System.Collections.Generic.List<ClienteEmail>());

                lstTelefonos.ItemsSource = Telefonos;
                lstEmails.ItemsSource = Emails;

                if (cliente.Direccion != null)
                {
                    cmbPais.SelectedItem = cliente.Direccion.Pais;
                    cmbProvincia.SelectedItem = cliente.Direccion.Provincia;
                    cmbCiudad.SelectedItem = cliente.Direccion.Ciudad;
                    txtCalle.Text = cliente.Direccion.Calle;
                    txtCodigoPostal.Text = cliente.Direccion.CodigoPostal;
                }
            }
            else
            {
                Cliente = new Cliente();
                Telefonos = new ObservableCollection<ClienteTelefono>();
                Emails = new ObservableCollection<ClienteEmail>();

                lstTelefonos.ItemsSource = Telefonos;
                lstEmails.ItemsSource = Emails;
            }

            if (soloLectura)
            {
                txtNombre.IsReadOnly = true;

                txtNuevoTelefono.IsEnabled = false;
                btnAgregarTelefono.IsEnabled = false;
                lstTelefonos.IsEnabled = false;

                txtNuevoEmail.IsEnabled = false;
                btnAgregarEmail.IsEnabled = false;
                lstEmails.IsEnabled = false;

                cmbPais.IsEnabled = false;
                cmbProvincia.IsEnabled = false;
                cmbCiudad.IsReadOnly = true;
                txtCalle.IsReadOnly = true;
                txtCodigoPostal.IsReadOnly = true;

                btnGuardar.IsEnabled = false;
            }
        }

        private void cmbPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var paisSeleccionado = cmbPais.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(paisSeleccionado))
            {
                cmbProvincia.ItemsSource = null;
                return;
            }

            // Formatear el nombre para el método (quita espacios y caracteres especiales)
            string methodName = "Load" + paisSeleccionado.Replace(" ", "") + "LocationData";

            var method = typeof(CountryLoader).GetMethod(methodName);
            if (method != null)
            {
                var countryData = method.Invoke(null, null);
                if (countryData != null)
                {
                    // Asumo que el resultado tiene una propiedad States con Name
                    var statesProperty = countryData.GetType().GetProperty("States");
                    if (statesProperty != null)
                    {
                        var states = statesProperty.GetValue(countryData) as System.Collections.IEnumerable;
                        if (states != null)
                        {
                            var listaEstados = new List<string>();
                            foreach (var state in states)
                            {
                                var nameProp = state.GetType().GetProperty("Name");
                                if (nameProp != null)
                                {
                                    var name = nameProp.GetValue(state) as string;
                                    if (!string.IsNullOrEmpty(name))
                                        listaEstados.Add(name);
                                }
                            }
                            listaEstados.Sort();
                            cmbProvincia.ItemsSource = listaEstados;
                            if (listaEstados.Count > 0)
                                cmbProvincia.SelectedIndex = 0;
                            return;
                        }
                    }
                }
            }

            cmbProvincia.ItemsSource = null;
        }
        private void cmbProvincia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var paisSeleccionado = cmbPais.SelectedItem as string;
            var provinciaSeleccionada = cmbProvincia.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(paisSeleccionado) || string.IsNullOrWhiteSpace(provinciaSeleccionada))
            {
                cmbCiudad.ItemsSource = null;
                return;
            }

            string methodName = "Load" + paisSeleccionado.Replace(" ", "") + "LocationData";
            var method = typeof(CountryLoader).GetMethod(methodName);

            if (method != null)
            {
                var countryData = method.Invoke(null, null);
                if (countryData != null)
                {
                    var statesProperty = countryData.GetType().GetProperty("States");
                    if (statesProperty != null)
                    {
                        var states = statesProperty.GetValue(countryData) as System.Collections.IEnumerable;
                        if (states != null)
                        {
                            foreach (var state in states)
                            {
                                var nameProp = state.GetType().GetProperty("Name");
                                if (nameProp != null)
                                {
                                    var stateName = nameProp.GetValue(state) as string;
                                    if (stateName == provinciaSeleccionada)
                                    {
                                        // Ahora obtenemos las Provinces dentro del State (puede ser Provinces o States según estructura)
                                        var provincesProperty = state.GetType().GetProperty("Provinces");
                                        if (provincesProperty != null)
                                        {
                                            var provinces = provincesProperty.GetValue(state) as System.Collections.IEnumerable;
                                            if (provinces != null)
                                            {
                                                var listaCiudades = new List<string>();

                                                foreach (var province in provinces)
                                                {
                                                    // Por cada province obtenemos Communities
                                                    var communitiesProperty = province.GetType().GetProperty("Communities");
                                                    if (communitiesProperty != null)
                                                    {
                                                        var communities = communitiesProperty.GetValue(province) as System.Collections.IEnumerable;
                                                        if (communities != null)
                                                        {
                                                            foreach (var community in communities)
                                                            {
                                                                // Por cada community obtenemos Places
                                                                var placesProperty = community.GetType().GetProperty("Places");
                                                                if (placesProperty != null)
                                                                {
                                                                    var places = placesProperty.GetValue(community) as System.Collections.IEnumerable;
                                                                    if (places != null)
                                                                    {
                                                                        foreach (var place in places)
                                                                        {
                                                                            var placeNameProp = place.GetType().GetProperty("Name");
                                                                            if (placeNameProp != null)
                                                                            {
                                                                                var placeName = placeNameProp.GetValue(place) as string;
                                                                                if (!string.IsNullOrEmpty(placeName) && !listaCiudades.Contains(placeName))
                                                                                    listaCiudades.Add(placeName);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                listaCiudades.Sort();
                                                cmbCiudad.ItemsSource = listaCiudades;
                                                if (listaCiudades.Count > 0)
                                                    cmbCiudad.SelectedIndex = 0;
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            cmbCiudad.ItemsSource = null;
        }


        private void AgregarTelefono_Click(object sender, RoutedEventArgs e)
        {
            var numero = txtNuevoTelefono.Text.Trim();
            if (!string.IsNullOrEmpty(numero))
            {
                Telefonos.Add(new ClienteTelefono { Numero = numero });
                txtNuevoTelefono.Clear();
            }
        }

        private void EliminarTelefono_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ClienteTelefono tel)
            {
                Telefonos.Remove(tel);
            }
        }

        private void AgregarEmail_Click(object sender, RoutedEventArgs e)
        {
            var email = txtNuevoEmail.Text.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                Emails.Add(new ClienteEmail { Email = email });
                txtNuevoEmail.Clear();
            }
        }

        private void EliminarEmail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ClienteEmail email)
            {
                Emails.Remove(email);
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre del cliente.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Cliente.Nombre = txtNombre.Text.Trim();
            Cliente.Telefonos = Telefonos.ToList();
            Cliente.Emails = Emails.ToList();

            if (Cliente.Direccion == null)
                Cliente.Direccion = new Direccion();

            Cliente.Direccion.Pais = cmbPais.SelectedItem as string ?? string.Empty;
            Cliente.Direccion.Provincia = cmbProvincia.SelectedItem as string ?? string.Empty;
            Cliente.Direccion.Ciudad = cmbCiudad.SelectedItem as string ?? string.Empty;
            Cliente.Direccion.Calle = txtCalle.Text.Trim();
            Cliente.Direccion.CodigoPostal = txtCodigoPostal.Text.Trim();
            _clienteRepo.Insert(Cliente);
            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
