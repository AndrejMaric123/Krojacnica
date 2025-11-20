using Krojacnica.Models;
using Krojacnica.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Krojacnica.Views
{
    public partial class OtkupMaterijalaWindow : Window
    {
        private ObservableCollection<DobavljacViewModel> dobavljaci;
        private ObservableCollection<OtkupMaterijalStavkaViewModel> materijali;
        public ObservableCollection<BojaViewModel> Boje { get; set; }
        private ObservableCollection<OtkupMaterijalStavkaViewModel> otkupStavke = new();

        public OtkupMaterijalaWindow()
        {
            InitializeComponent();
            LoadDobavljaci();
            LoadBoje();
        }

        private void ShowSnackbar(string message, int durationMs = 3000)
        {
            if (SnackbarHost.MessageQueue == null)
                SnackbarHost.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(durationMs));

            SnackbarHost.MessageQueue.Enqueue(message);
        }

        private void LoadBoje()
        {
            using var context = DbContextFactory.Create();
            Boje = new ObservableCollection<BojaViewModel>(
                context.bojas.Select(b => new BojaViewModel(b.naziv, b.hex_code)).ToList()
            );
        }

        private void LoadDobavljaci()
        {
            using var context = DbContextFactory.Create();
            dobavljaci = new ObservableCollection<DobavljacViewModel>(
                context.dobavljacs
                       .Include(d => d.individualni)
                       .Include(d => d.preduzece)
                       .Select(d => new DobavljacViewModel(d))
                       .ToList()
            );
            cbDobavljaci.ItemsSource = dobavljaci;
        }

        private void cbDobavljaci_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDobavljaci.SelectedItem is not DobavljacViewModel selected) return;

            using var context = DbContextFactory.Create();
            var materijalDobavljaci = context.materijal_dobavljacs
                .Include(md => md.materijal)
                .Where(md => md.dobavljac_sifra == selected.Sifra)
                .ToList();

            materijali = new ObservableCollection<OtkupMaterijalStavkaViewModel>(
                materijalDobavljaci.Select(md => new OtkupMaterijalStavkaViewModel(md))
            );

            dgMaterijali.ItemsSource = materijali;
        }

        private void AddToOtkup_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaterijali.SelectedItem is not OtkupMaterijalStavkaViewModel selected)
                return;

            var window = new UnosStavkeWindow(Boje);
            if (window.ShowDialog() == true)
            {
                var novaStavka = new OtkupMaterijalStavkaViewModel(new materijal_dobavljac
                {
                    id = selected.MaterijalDobavljacId,
                    cijena = selected.Cijena,
                    materijal = new materijal { naziv = selected.MaterijalNaziv }
                })
                {
                    Kolicina = window.Kolicina,
                    BojaHexCode = window.HexBoja
                };

                otkupStavke.Add(novaStavka);
                dgOtkup.ItemsSource = null;
                dgOtkup.ItemsSource = otkupStavke;
                UpdateUkupno();
            }
        }

        private void RemoveFromOtkup_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgOtkup.SelectedItems.Cast<OtkupMaterijalStavkaViewModel>().ToList();
            foreach (var s in selected) otkupStavke.Remove(s);

            dgOtkup.ItemsSource = null;
            dgOtkup.ItemsSource = otkupStavke;
            UpdateUkupno();
        }

        private void UpdateUkupno()
        {
            txtUkupno.Text = otkupStavke.Sum(s => s.Ukupno).ToString("0.00");
        }

        private void SaveOtkup_Click(object sender, RoutedEventArgs e)
        {
            if (cbDobavljaci.SelectedItem is not DobavljacViewModel selectedDobavljac)
            {
                ShowSnackbar("Odaberite dobavljača.");
                return;
            }

            if (!otkupStavke.Any())
            {
                ShowSnackbar("Dodajte barem jednu stavku.");
                return;
            }

            using var context = DbContextFactory.Create();
            var noviOtkup = new otkup
            {
                datum = DateOnly.FromDateTime(DateTime.Now),
                dobavljac_sifra = selectedDobavljac.Sifra
            };
            context.otkups.Add(noviOtkup);
            context.SaveChanges();

            foreach (var stavka in otkupStavke)
            {
                var novaStavka = new otkup_stavka
                {
                    otkup_broj_potvrde = noviOtkup.broj_potvrde,
                    materijal_dobavljac_id = stavka.MaterijalDobavljacId,
                    kolicina = stavka.Kolicina,
                    boja_hex_code = stavka.BojaHexCode
                };
                context.otkup_stavkas.Add(novaStavka);

                var materijalId = context.materijal_dobavljacs
                    .Where(md => md.id == stavka.MaterijalDobavljacId)
                    .Select(md => md.materijal_id)
                    .FirstOrDefault();

                if (materijalId == 0) continue;

                var postojecaZaliha = context.materijal_zalihas
                    .FirstOrDefault(z => z.materijal_id == materijalId && z.boja_hex_code == stavka.BojaHexCode);

                if (postojecaZaliha != null)
                {
                    postojecaZaliha.dostupna_kolicina += stavka.Kolicina;
                    context.Entry(postojecaZaliha).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    var novaZaliha = new materijal_zaliha
                    {
                        materijal_id = materijalId,
                        boja_hex_code = stavka.BojaHexCode,
                        dostupna_kolicina = stavka.Kolicina
                    };
                    context.materijal_zalihas.Add(novaZaliha);
                }
            }

            context.SaveChanges();

            ShowSnackbar("Otkup uspješno sačuvan!");
            Close();
        }
    }
}
