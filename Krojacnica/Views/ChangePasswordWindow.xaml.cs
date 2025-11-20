using Krojacnica.Helpers;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class ChangePasswordWindow : Window
    {
        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        public ChangePasswordWindow()
        {
            InitializeComponent();

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text?.Trim() ?? "";
            string oldPassword = PwdOldPassword.Password?.Trim() ?? "";
            string newPassword = PwdNewPassword.Password?.Trim() ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                SnackbarMessageQueue.Enqueue("Sva polja su obavezna!");
                return;
            }

            try
            {
                using var context = DbContextFactory.Create();

                var zaposleni = context.zaposlenis
                    .FirstOrDefault(z => z.korisnicko_ime == username);

                if (zaposleni == null)
                {
                    SnackbarMessageQueue.Enqueue("Korisnik nije pronađen!");
                    return;
                }

                if (!PasswordHelper.VerifyPassword(oldPassword, zaposleni.lozinka))
                {
                    SnackbarMessageQueue.Enqueue("Stara lozinka nije ispravna!");
                    return;
                }

                zaposleni.lozinka = PasswordHelper.HashPassword(newPassword, out _);
                context.SaveChanges();

                SnackbarMessageQueue.Enqueue("Lozinka je uspješno promijenjena!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                SnackbarMessageQueue.Enqueue("Greška: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);

            // Fokus na prvo polje
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TxtUsername.Focus();
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
    }
}
