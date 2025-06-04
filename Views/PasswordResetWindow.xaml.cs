using System.Windows;

namespace SmartGestionApp.Views
{
    public partial class PasswordResetWindow : Window
    {
        public string? NewPassword { get; private set; }

        public PasswordResetWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var pass1 = NewPasswordBox.Password;
            var pass2 = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(pass1) || string.IsNullOrWhiteSpace(pass2))
            {
                MessageBox.Show("Por favor, complete ambos campos de contraseña.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pass1 != pass2)
            {
                MessageBox.Show("Las contraseñas no coinciden. Intente nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewPassword = pass1;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
