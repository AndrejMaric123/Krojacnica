using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class IzdajRacunWindow : Window
    {
        private readonly narudzba _narudzba;
        private readonly klijent _klijent;

        public racun IzdaniRacun { get; private set; }


        public decimal UkupanIznos { get; private set; }
        public bool RacunIzdana { get; private set; }

        private SnackbarMessageQueue _messageQueue;

        public IzdajRacunWindow(narudzba narudzba, klijent klijent)
        {
            InitializeComponent();

            _narudzba = narudzba;
            _klijent = klijent;

            _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = _messageQueue;

            // proračun iznosa
            using var db = DbContextFactory.Create();
            UkupanIznos = db.stavka_narudzbes
                .Where(s => s.narudzba_broj_narudzbe == narudzba.broj_narudzbe)
                .Sum(s => s.Cijena * s.kolicina);

            txtIznos.Text = UkupanIznos.ToString("0.00") + " KM";
        }


        private void BtnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (cbNacinPlacanja.SelectedItem is not ComboBoxItem item)
            {
                _messageQueue.Enqueue("Odaberite način plaćanja!");

                return;
            }

            string nacin = item.Content.ToString();

            using var db = DbContextFactory.Create();

            var racun = new racun
            {
                datum_izdavanja = DateOnly.FromDateTime(DateTime.Now),
                klijent_osoba_id = _klijent.osoba_id,
                zaposleni_osoba_id = 1, 
                narudzba_broj_narudzbe = _narudzba.broj_narudzbe,
                ukupan_iznos = UkupanIznos,
                NačinPlaćanja = nacin
            };

            db.racuns.Add(racun);
            db.SaveChanges();

            IzdaniRacun = racun; // <-- ovo je ključno
            RacunIzdana = true;
            DialogResult = true;
            Close();


          
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
            {
                sb.Begin(CardBorder);
            }
        }

        private void BtnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
