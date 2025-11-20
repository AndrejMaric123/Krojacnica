using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Krojacnica.Views
{
    public partial class KlijentDetaljiWindow : Window
    {
        private int _klijentId;

        public ObservableCollection<NarudzbaViewModel> Narudzbe { get; set; } = new();
        public ObservableCollection<RacunViewModel> Racuni { get; set; } = new();

        public ObservableCollection<MjereViewModel> Mjere { get; set; } = new();

        private SnackbarMessageQueue _messageQueue;

        public KlijentDetaljiWindow(int klijentId)
        {
            InitializeComponent();
            _klijentId = klijentId;

            _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = _messageQueue;

            dgNarudzbe.ItemsSource = Narudzbe;
            dgRacuni.ItemsSource = Racuni;

            LoadKlijent();
            LoadNarudzbe();
            LoadRacune();
            LoadMjere();
        }


        private void LoadMjere()
        {
            using var db = DbContextFactory.Create();

            Mjere.Clear();
            var list = db.mjeres
                         .Where(m => m.klijent_osoba_id == _klijentId)
                         .OrderByDescending(m => m.datum)
                         .Select(m => new MjereViewModel
                         {
                             Datum = m.datum,
                             SirinaRamena = m.sirina_ramena,
                             ObimGrudi = m.obim_grudi,
                             ObimStruka = m.obim_struka,
                             ObimKukova = m.obim_kukova,
                             ObimBokova = m.obim_bokova,
                             Duzina = m.duzina
                         })
                         .ToList();

            foreach (var m in list)
                Mjere.Add(m);

            dgMjere.ItemsSource = Mjere;
        }

        private void BtnDetaljiMjere_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.Tag is not DateOnly datum)
                return;

            var mjera = Mjere.FirstOrDefault(m => m.Datum == datum);
            if (mjera == null) return;

            var win = new PregledMjeraWindow(mjera) { Owner = this };
            win.ShowDialog();
        }

        private void BtnDodajMjere_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddMeasurementsWindow(_klijentId)
            {
                Owner = this
            };

            if (win.ShowDialog() == true)
            {
                // Dohvati zadnju unesenu mjeru iz baze (najjednostavnije)
                using var db = DbContextFactory.Create();
                var m = db.mjeres
                          .Where(x => x.klijent_osoba_id == _klijentId)
                          .OrderByDescending(x => x.datum)
                          .FirstOrDefault();

                if (m != null)
                {
                    Mjere.Add(new MjereViewModel
                    {
                        Datum = m.datum,
                        SirinaRamena = m.sirina_ramena,
                        ObimGrudi = m.obim_grudi,
                        ObimStruka = m.obim_struka,
                        ObimKukova = m.obim_kukova,
                        ObimBokova = m.obim_bokova,
                        Duzina = m.duzina
                    });
                }
            }
        }




        private void LoadKlijent()
        {
            using var db = DbContextFactory.Create();

            var k = db.klijents
                      .Include(x => x.osoba)
                      .FirstOrDefault(x => x.osoba_id == _klijentId);

            if (k?.osoba != null)
                txtHeader.Text = $"{k.osoba.ime} {k.osoba.prezime}";
        }

        private void LoadNarudzbe()
        {
            using var db = DbContextFactory.Create();

            Narudzbe.Clear();
            var list = db.narudzbas
                         .Where(x => x.klijent_osoba_id == _klijentId)
                         .OrderByDescending(x => x.datum)
                         .Select(n => new NarudzbaViewModel
                         {
                             BrojNarudzbe = n.broj_narudzbe,
                             Datum = n.datum,
                             Status = n.status_narudzbe_naziv,
                             UkupnaCijena = n.stavka_narudzbes.Sum(s => s.Cijena * s.kolicina),
                             MozeIzdatiRacun = !db.racuns.Any(r => r.narudzba_broj_narudzbe == n.broj_narudzbe)
                         })
                         .ToList();

            foreach (var n in list)
                Narudzbe.Add(n);
        }


        private void LoadRacune()
        {
            using var db = DbContextFactory.Create();

            Racuni.Clear();
            var list = db.racuns
                         .Include(r => r.klijent_osoba)
                             .ThenInclude(k => k.osoba)
                         .Where(x => x.klijent_osoba_id == _klijentId)
                         .OrderByDescending(x => x.datum_izdavanja)
                         .Select(r => new RacunViewModel
                         {
                             BrojRacuna = r.broj_racuna,
                             DatumIzdavanja = r.datum_izdavanja,
                             UkupanIznos = r.ukupan_iznos,
                             NacinPlacanja = r.NačinPlaćanja,
                             NarudzbaBroj = r.narudzba_broj_narudzbe
                         })
                         .ToList();

            foreach (var r in list)
                Racuni.Add(r);

            txtUkupno.Text = Racuni.Sum(r => r.UkupanIznos).ToString("0.00") + " KM";
        }

        // Otvaranje detalja računa iz DataGrida racuni
        private void BtnPregledajRacun_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.Tag is not int brojRacuna)
                return;

            using var db = DbContextFactory.Create();

            var racunEntity = db.racuns
                .Include(r => r.klijent_osoba)
                .Include(r => r.narudzba_broj_narudzbeNavigation)
                .FirstOrDefault(r => r.broj_racuna == brojRacuna);

            if (racunEntity == null)
                return;

            var win = new PregledRacunaWindow(racunEntity)
            {
                Owner = this
            };
            win.ShowDialog();
        }

        // Izdavanje računa
        private void BtnIzdajRacun_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.Tag is not int brojNarudzbe)
                return;

            using var db = DbContextFactory.Create();

            var narudzba = db.narudzbas.First(n => n.broj_narudzbe == brojNarudzbe);
            var klijent = db.klijents.First(k => k.osoba_id == _klijentId);

            var win = new IzdajRacunWindow(narudzba, klijent)
            {
                Owner = this
            };

            if (win.ShowDialog() == true && win.RacunIzdana)
            {
                MessageBox.Show("Račun uspješno izdat!", "Info");

                // Dodaj novi račun u ObservableCollection (automatski osvježava UI)
                Racuni.Add(new RacunViewModel
                {
                    BrojRacuna = win.IzdaniRacun.broj_racuna,
                    DatumIzdavanja = win.IzdaniRacun.datum_izdavanja,
                    UkupanIznos = win.IzdaniRacun.ukupan_iznos,
                    NacinPlacanja = win.IzdaniRacun.NačinPlaćanja,
                    NarudzbaBroj = win.IzdaniRacun.narudzba_broj_narudzbe
                });

                // Refresh ukupno
                txtUkupno.Text = Racuni.Sum(r => r.UkupanIznos).ToString("0.00") + " KM";

                // Promijeni status narudžbe u ObservableCollection
                var nar = Narudzbe.FirstOrDefault(n => n.BrojNarudzbe == brojNarudzbe);
                if (nar != null)
                {
                    nar.MozeIzdatiRacun = false;
                    nar.Status = narudzba.status_narudzbe_naziv;
                }
                
            }
        }

        // Promjena statusa narudžbe
        private void BtnPromijeniStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.Tag is not int brojNarudzbe)
                return;

            using var db = DbContextFactory.Create();

            var narudzba = db.narudzbas.FirstOrDefault(n => n.broj_narudzbe == brojNarudzbe);
            if (narudzba == null) return;

            var statusi = db.status_narudzbes.ToList();

            var win = new PromijeniStatusWindow(narudzba, statusi)
            {
                Owner = this
            };

            if (win.ShowDialog() == true)
            {
                _messageQueue.Enqueue($"Status narudžbe #{brojNarudzbe} promijenjen u {narudzba.status_narudzbe_naziv}");


                // Update ObservableCollection
                var nar = Narudzbe.FirstOrDefault(n => n.BrojNarudzbe == brojNarudzbe);
                if (nar != null)
                    nar.Status = narudzba.status_narudzbe_naziv;
            }
        }

        private void BtnOtvoriNarudzbu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.Tag is not int brojNarudzbe)
                return;

            using var db = DbContextFactory.Create();

            var narudzbaEntity = db.narudzbas
                                    .Include(n => n.stavka_narudzbes)
                                        .ThenInclude(s => s.ponuda)
                                            .ThenInclude(p => p.artikal)
                                    .Include(n => n.stavka_narudzbes)
                                        .ThenInclude(s => s.ponuda)
                                            .ThenInclude(p => p.usluga)
                                    .FirstOrDefault(n => n.broj_narudzbe == brojNarudzbe);

            if (narudzbaEntity == null) return;

            var win = new PregledNarudzbeWindow(narudzbaEntity)
            {
                Owner = this
            };
            win.ShowDialog();
        }

    }
}
