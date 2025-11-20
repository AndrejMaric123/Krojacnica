using Krojacnica.Helpers;
using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Krojacnica.Views
{
    public partial class PregledKlijenataWindow : Window
    {
        public ObservableCollection<KlijentViewModel> Klijenti { get; set; } = new();

        public PregledKlijenataWindow()
        {
            InitializeComponent();

            dgKlijenti.ItemsSource = Klijenti;

            // Inicijalizacija SnackbarMessageQueue
            if (SnackbarHost.MessageQueue == null)
                SnackbarHost.MessageQueue = new SnackbarMessageQueue(System.TimeSpan.FromSeconds(3));

            UcitajKlijente();
        }

        private void UcitajKlijente()
        {
            using var db = DbContextFactory.Create();

            Klijenti.Clear();

            var klijenti = db.klijents
                .Include(k => k.osoba)
                .Include(k => k.narudzbas)
                .Select(k => new KlijentViewModel
                {
                    Id = k.osoba_id,
                    Ime = k.osoba.ime,
                    Prezime = k.osoba.prezime,
                    Telefon = k.osoba.telefon,
                    Email = k.osoba.email,
                    BrojNarudzbi = k.narudzbas.Count
                })
                .OrderBy(k => k.Prezime)
                .ToList();

            foreach (var k in klijenti)
                Klijenti.Add(k);

            
        }

        private void BtnDetalji_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int klijentId)
            {
                var win = new KlijentDetaljiWindow(klijentId)
                {
                    Owner = this
                };
                win.Show();

                
            }
        }

        private void AddKlijent_Click(object sender, RoutedEventArgs e)
        {
            var win = new RegisterClientWindow
            {
                Owner = this
            };

            if (win.ShowDialog() == true)
            {
                // Nakon što se registruje novi klijent, dodaj ga direktno u kolekciju
                using var db = DbContextFactory.Create();
                var osoba = db.osobas.OrderByDescending(o => o.id).FirstOrDefault();
                if (osoba != null)
                {
                    var klijent = db.klijents.FirstOrDefault(k => k.osoba_id == osoba.id);
                    if (klijent != null)
                    {
                        Klijenti.Add(new KlijentViewModel
                        {
                            Id = klijent.osoba_id,
                            Ime = osoba.ime,
                            Prezime = osoba.prezime,
                            Email = osoba.email,
                            Telefon = osoba.telefon,
                            BrojNarudzbi = klijent.narudzbas.Count
                        });

                        

                        SnackbarHost.MessageQueue.Enqueue($"Klijent {osoba.ime} {osoba.prezime} uspješno dodan!");
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = txtSearch.Text.Trim().ToLower();
            dgKlijenti.ItemsSource = string.IsNullOrEmpty(query)
                ? Klijenti
                : new ObservableCollection<KlijentViewModel>(
                    Klijenti.Where(k =>
                        k.Ime.ToLower().Contains(query) ||
                        k.Prezime.ToLower().Contains(query) ||
                        (k.Telefon?.ToLower().Contains(query) ?? false) ||
                        (k.Email?.ToLower().Contains(query) ?? false))
                  );
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
