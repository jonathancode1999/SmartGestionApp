using SmartGestionApp.Data.Repositories;
using SmartGestionApp.Helpers;
using SmartGestionApp.Properties;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
using SmartGestionApp.Data; // Para InputBox, aunque es raro en WPF, funciona.

namespace SmartGestionApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UsuarioRepository _usuarioRepo;

        public LoginWindow()
        {
            InitializeComponent();
            _usuarioRepo = new UsuarioRepository(DatabaseManager.GetConnection().ConnectionString);

            // Intentar login automático cuando la ventana ya esté cargada
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.Email) &&
                !string.IsNullOrWhiteSpace(Settings.Default.PasswordHash))
            {
                AutoLogin();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingrese email y contraseña.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuario = _usuarioRepo.GetAll().FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash != null &&
                SecurityHelper.VerifyPassword(password, u.PasswordHash));

            if (usuario is not null && usuario.Activo)
            {
                if (RememberMeCheckBox.IsChecked == true)
                {
                    Settings.Default.Email = email;
                    Settings.Default.PasswordHash = usuario.PasswordHash!;
                    Settings.Default.Save();
                }

                MessageBox.Show($"¡Bienvenido, {usuario.Nombre}!", "Inicio de sesión",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Credenciales inválidas o usuario inactivo.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Por favor, ingrese su email para recuperar la contraseña.", "Recuperar contraseña",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuario = _usuarioRepo.GetAll().FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

            if (usuario == null)
            {
                MessageBox.Show("No se encontró ningún usuario con ese email.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Solicitar nombre para verificación de identidad
            string nombreIngresado = Interaction.InputBox(
                "Por favor, ingrese su nombre para verificar su identidad:",
                "Verificación de identidad", "");

            if (string.IsNullOrWhiteSpace(nombreIngresado))
            {
                MessageBox.Show("Debe ingresar su nombre para continuar con la recuperación.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.Equals(usuario.Nombre, nombreIngresado.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("El nombre ingresado no coincide con el registrado para este email.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Abrir ventana para nueva contraseña
            var passwordResetWindow = new PasswordResetWindow { Owner = this };

            bool? result = passwordResetWindow.ShowDialog();

            if (result == true)
            {
                string nuevaPass = passwordResetWindow.NewPassword!;
                usuario.PasswordHash = SecurityHelper.HashPassword(nuevaPass);
                _usuarioRepo.Update(usuario);

                MessageBox.Show("Contraseña actualizada correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Operación cancelada.", "Cancelado",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Permite iniciar sesión presionando Enter desde Email o PasswordBox
        private void LoginField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(this, new RoutedEventArgs());
            }
        }

        private void AutoLogin()
        {
            var usuario = _usuarioRepo.GetAll().FirstOrDefault(u =>
                string.Equals(u.Email, Settings.Default.Email, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == Settings.Default.PasswordHash &&
                u.Activo);

            if (usuario != null)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }
    }
}
