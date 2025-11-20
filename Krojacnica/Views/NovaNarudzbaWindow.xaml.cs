using Krojacnica.Models;
using Krojacnica.ViewModel;
using Krojacnica.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Krojacnica.Views
{
    public partial class NovaNarudzbaWindow : Window
    {
        private ObservableCollection<StavkaNarudzbeViewModel> stavke = new();
        private decimal ukupno = 0;

        public NovaNarudzbaWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void ShowSnackbar(string message, int durationMs = 3000)
        {
            if (SnackbarHost.MessageQueue is null)
                SnackbarHost.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(durationMs));

            SnackbarHost.MessageQueue.Enqueue(message);
        }

        private void LoadData()
        {
            using var db = DbContextFactory.Create();

            // Klijenti
            var klijenti = db.klijents
                .Select(k => new
                {
                    k.osoba_id,
                    ImePrezime = k.osoba.ime + " " + k.osoba.prezime
                })
                .ToList();
            cmbKlijent.ItemsSource = klijenti;

            // Ponude
            var ponude = db.ponuda
                .Include(p => p.artikal)
                .Include(p => p.usluga)
                .Select(p => new PonudaViewModel
                {
                    Id = p.id,
                    Naziv = p.artikal != null ? p.artikal.naziv : p.usluga!.naziv,
                    Tip = p.artikal != null ? "Artikal" : "Usluga",
                    JedinicnaCijena = p.jedinicna_cijena,
                    Artikal = p.artikal,
                    Usluga = p.usluga
                })
                .ToList();

            cmbPonuda.ItemsSource = ponude;
            cmbPonuda.DisplayMemberPath = "Naziv";

            txtDatum.Text = DateTime.Now.ToShortDateString();
            txtStatus.Text = "U toku";

            dgStavke.ItemsSource = stavke;
        }

        private void BtnDodajStavku_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPonuda.SelectedItem is null || !int.TryParse(txtKolicina.Text, out int kolicina))
            {
                ShowSnackbar("Odaberite ponudu i unesite validnu količinu!");
                return;
            }

            if (cmbMjere.SelectedItem == null)
            {
                ShowSnackbar("Odaberite mjere klijenta!");
                return;
            }

            dynamic ponuda = cmbPonuda.SelectedItem;

            DateOnly odabranaMjeraDatum = (DateOnly)cmbMjere.SelectedValue;
            int klijentId = (int)cmbKlijent.SelectedValue;

            using var db = DbContextFactory.Create();
            var mjere = db.mjeres
                .FirstOrDefault(m => m.klijent_osoba_id == klijentId && m.datum == odabranaMjeraDatum);

            if (mjere == null)
            {
                ShowSnackbar("Odabrana mjera nije pronađena u bazi!");
                return;
            }

            double potrosnja = ((mjere.obim_grudi + mjere.obim_struka + mjere.obim_kukova + mjere.duzina * 2) / 100.0) * kolicina;

            var stavka = new StavkaNarudzbeViewModel
            {
                PonudaId = ponuda.Id,
                MaterijalId = ponuda.Artikal?.materijal_id ?? 0,
                Naziv = ponuda.Naziv,
                Tip = ponuda.Tip,
                Cijena = ponuda.JedinicnaCijena,
                Kolicina = kolicina,
                BojaHexCode = cmbBoja.SelectedValue?.ToString(),
                Ukupno = ponuda.JedinicnaCijena * kolicina,
                PotrosnjaMaterijala = potrosnja,
                ArtikalEntity = ponuda.Artikal,
                UslugaEntity = ponuda.Usluga
            };

            stavke.Add(stavka);
            ukupno = stavke.Sum(s => s.Ukupno);
            txtUkupno.Text = $"{ukupno:C}";
        }

        private void BtnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (cmbKlijent.SelectedItem is null || !stavke.Any())
            {
                ShowSnackbar("Odaberite klijenta i dodajte bar jednu stavku!");
                return;
            }

            if (cmbMjere.SelectedItem is null)
            {
                ShowSnackbar("Odaberite mjere klijenta!");
                return;
            }

            int klijentId = (int)cmbKlijent.SelectedValue;
            DateOnly odabranaMjeraDatum = (DateOnly)cmbMjere.SelectedValue;

            using var db = DbContextFactory.Create();
            var mjere = db.mjeres
                .FirstOrDefault(m => m.klijent_osoba_id == klijentId && m.datum == odabranaMjeraDatum);

            if (mjere == null)
            {
                ShowSnackbar("Odabrana mjera nije pronađena u bazi!");
                return;
            }

            var narudzba = new narudzba
            {
                datum = DateOnly.FromDateTime(DateTime.Now),
                klijent_osoba_id = klijentId,
                zaposleni_osoba_id = Krojacnica.Properties.Settings.Default.LoggedUserId,
                status_narudzbe_naziv = "U toku"
            };

            db.narudzbas.Add(narudzba);
            db.SaveChanges();

            sbyte redniBroj = 1;

            foreach (var s in stavke)
            {
                db.stavka_narudzbes.Add(new stavka_narudzbe
                {
                    redni_broj = redniBroj++,
                    kolicina = (sbyte)s.Kolicina,
                    Cijena = s.Cijena,
                    ponuda_id = s.PonudaId,
                    narudzba_broj_narudzbe = narudzba.broj_narudzbe
                });

                double potrosnjaPoArtiklu = (mjere.obim_grudi + mjere.obim_struka + mjere.obim_kukova + mjere.duzina * 2) / 100.0;
                double ukupnaPotrosnja = potrosnjaPoArtiklu * s.Kolicina;

                var materijalZaliha = db.materijal_zalihas
                    .FirstOrDefault(z => z.materijal_id == s.MaterijalId && z.boja_hex_code == s.BojaHexCode);

                if (materijalZaliha != null)
                {
                    materijalZaliha.dostupna_kolicina -= (int)Math.Ceiling(ukupnaPotrosnja);
                    if (materijalZaliha.dostupna_kolicina < 0)
                        materijalZaliha.dostupna_kolicina = 0;
                }
            }

            db.SaveChanges();

            ShowSnackbar("Narudžba je uspješno sačuvana i potrošnja materijala ažurirana!");
            DialogResult = true;
            Close();
        }

        private void cmbKlijent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbKlijent.SelectedItem == null) return;

            int klijentId = (int)cmbKlijent.SelectedValue;

            using var db = DbContextFactory.Create();
            var mjereList = db.mjeres
                .Where(m => m.klijent_osoba_id == klijentId)
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

            cmbMjere.ItemsSource = mjereList;
            cmbMjere.DisplayMemberPath = "Datum";
            cmbMjere.SelectedValuePath = "Datum";
        }

        private void cmbPonuda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPonuda.SelectedItem is not PonudaViewModel ponuda)
                return;

            if (ponuda.Artikal == null)
            {
                cmbBoja.ItemsSource = null;
                return;
            }

            int materijalId = ponuda.Artikal.materijal_id;

            using var db = DbContextFactory.Create();
            var boje = db.materijal_zalihas
                .Where(z => z.materijal_id == materijalId)
                .Join(db.bojas,
                      z => z.boja_hex_code,
                      b => b.hex_code,
                      (z, b) => new { Naziv = b.naziv, HexCode = b.hex_code })
                .ToList();

            cmbBoja.ItemsSource = boje;
            if (boje.Any())
                cmbBoja.SelectedIndex = 0;

            cmbBoja.DisplayMemberPath = "Naziv";
            cmbBoja.SelectedValuePath = "HexCode";
        }

        private void BtnPrikaziMjere_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMjere.SelectedItem is MjereViewModel odabrana)
            {
                var wnd = new PregledMjeraWindow(odabrana);
                wnd.Owner = this;
                wnd.ShowDialog();
            }
            else
            {
                ShowSnackbar("Prvo odaberite datum mjerenja!");
            }
        }

        private void BtnUkloniStavku_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is StavkaNarudzbeViewModel stavka)
            {
                stavke.Remove(stavka);
                dgStavke.Items.Refresh();
                ukupno = stavke.Sum(s => s.Ukupno);
                txtUkupno.Text = $"{ukupno:C}";
            }
        }

        private void BtnEditStavka_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not StavkaNarudzbeViewModel stavka)
                return;

            if (stavka.ArtikalEntity == null)
            {
                ShowSnackbar("Ova stavka nije artikal i ne može se uređivati.");
                return;
            }

            using var db = DbContextFactory.Create();
            var artikal = db.artikals
                .Include(a => a.ponuda)
                .Include(a => a.materijal)
                .Include(a => a.boja_hex_codeNavigation)
                .FirstOrDefault(a => a.sifra_artikla == stavka.ArtikalEntity.sifra_artikla);

            if (artikal == null)
            {
                ShowSnackbar("Artikal nije pronađen u bazi.");
                return;
            }

            var prozor = new EditArtikalWindow(artikal, true)
            {
                Owner = this
            };
            prozor.ShowDialog();
        }
    }
}
