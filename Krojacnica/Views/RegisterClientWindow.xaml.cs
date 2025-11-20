using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class RegisterClientWindow : Window
    {
        private SnackbarMessageQueue _snackbarQueue;

        public RegisterClientWindow()
        {
            InitializeComponent();

            // Inicijalizacija snackbar-a
            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
            SnackbarHost.MessageQueue = _snackbarQueue;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string ime = txtFirstName.Text.Trim();
            string prezime = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefon = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(ime) || string.IsNullOrEmpty(prezime))
            {
                _snackbarQueue.Enqueue("Ime i prezime su obavezni!");
                return;
            }

            try
            {
                using var context = DbContextFactory.Create();

                var osoba = new osoba
                {
                    ime = ime,
                    prezime = prezime,
                    email = email,
                    telefon = telefon
                };

                context.osobas.Add(osoba);
                context.SaveChanges();

                var klijent = new klijent
                {
                    osoba_id = osoba.id
                };

                context.klijents.Add(klijent);
                context.SaveChanges();

                _snackbarQueue.Enqueue("Klijent uspješno registrovan!");

                this.DialogResult = true;
                this.Close();
                
            }
            catch (Exception ex)
            {
                _snackbarQueue.Enqueue($"Greška: {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
            {
                sb.Begin(CardBorder);
            }
        }
    }
}
