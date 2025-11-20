using Krojacnica.Helpers;
using Krojacnica.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class AddEmployeeWindow : Window
    {
        public AddEmployeeViewModel ViewModel { get; set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
            ViewModel = new AddEmployeeViewModel();
            DataContext = ViewModel;
        }

        private async void CreateEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            

            await Task.Delay(300); // mali vizuelni delay

            try
            {
                ViewModel.Password = PasswordBox.Password.Trim();

                // ---- VALIDACIJE ----
                if (string.IsNullOrWhiteSpace(ViewModel.Ime) ||
                    string.IsNullOrWhiteSpace(ViewModel.Prezime) ||
                    string.IsNullOrWhiteSpace(ViewModel.Username) ||
                    string.IsNullOrWhiteSpace(ViewModel.Password))
                {
                    ViewModel.SnackbarMessageQueue.Enqueue("Popunite sva polja!");
                    return;
                }

                if (!ViewModel.OdDatuma.HasValue || !ViewModel.DoDatuma.HasValue)
                {
                    ViewModel.SnackbarMessageQueue.Enqueue("Odaberite datume!");
                    return;
                }

                if (ViewModel.OdDatuma >= ViewModel.DoDatuma)
                {
                    ViewModel.SnackbarMessageQueue.Enqueue("Datum završetka mora biti poslije početnog!");
                    return;
                }

                using var context = DbContextFactory.Create();

                if (context.zaposlenis.Any(z => z.korisnicko_ime == ViewModel.Username))
                {
                    ViewModel.SnackbarMessageQueue.Enqueue("Korisničko ime već postoji!");
                    return;
                }

                // ---- KREIRANJE ----

                var osoba = new Models.osoba
                {
                    ime = ViewModel.Ime,
                    prezime = ViewModel.Prezime,
                    email = "",
                    telefon = ""
                };
                context.osobas.Add(osoba);
                context.SaveChanges();

                var hash = PasswordHelper.HashPassword(ViewModel.Password, out _);

                var zaposleni = new Models.zaposleni
                {
                    osoba_id = osoba.id,
                    korisnicko_ime = ViewModel.Username,
                    lozinka = hash
                };
                context.zaposlenis.Add(zaposleni);
                context.SaveChanges();

                int brojUgovora = GenerateContractNumber();

                var zaposlenje = new Models.zaposlenje
                {
                    broj_ugovora = brojUgovora,
                    od_datuma = DateOnly.FromDateTime(ViewModel.OdDatuma.Value),
                    do_datuma = DateOnly.FromDateTime(ViewModel.DoDatuma.Value),
                    osoba_id = osoba.id
                };
                context.zaposlenjes.Add(zaposlenje);
                context.SaveChanges();

                ViewModel.SnackbarMessageQueue.Enqueue(
                    $"Zaposleni {ViewModel.Ime} {ViewModel.Prezime} kreiran!\nUgovor: {brojUgovora}");

              
                this.DialogResult = true;

                
                this.Close();
            }
            finally
            {
                
            }
        }



        private int GenerateContractNumber()
        {
            using var context = DbContextFactory.Create();
            return context.zaposlenjes.Any()
                ? context.zaposlenjes.Max(z => z.broj_ugovora) + 1
                : 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)Application.Current.Resources["AppWindowEnterAnimation"];
            sb.Begin(this);

            TxtIme.Focus();
        }


    }
}
