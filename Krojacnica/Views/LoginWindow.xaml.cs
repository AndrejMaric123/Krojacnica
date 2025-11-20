using Krojacnica.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Krojacnica.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text?.Trim() ?? "";
            string password = PwdPassword.Password?.Trim() ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Unesite korisničko ime i lozinku!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = DbContextFactory.Create();

                var zaposleni = context.zaposlenis
                    .Include(z => z.admin)
                    .FirstOrDefault(z => z.korisnicko_ime == username);

                if (zaposleni == null || string.IsNullOrEmpty(zaposleni.lozinka) ||
                    !PasswordHelper.VerifyPassword(password, zaposleni.lozinka))
                {
                    MessageBox.Show("Neispravno korisničko ime ili lozinka!", "Neuspjeh", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ✅ Pamćenje sesije
                Properties.Settings.Default.LoggedUserId = zaposleni.osoba_id;
                Properties.Settings.Default.LoggedUserRole = zaposleni.admin != null ? "Admin" : "Employee";
                Properties.Settings.Default.RememberMe = ChkRemember.IsChecked == true;
                Properties.Settings.Default.Save();

                // ✅ Otvori odgovarajući prozor
                if (zaposleni.admin != null)
                {
                    var adminWindow = new AdminWindow(zaposleni);
                    adminWindow.Show();
                }
               

                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Greška pri povezivanju: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ForgotPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow();
            changePasswordWindow.Owner = this;
            changePasswordWindow.ShowDialog();
        }


        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LoggedUserId = 0;
            Properties.Settings.Default.LoggedUserRole = string.Empty;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();

            var login = new LoginWindow();
            login.Show();
            Close();
        }

    }
}
