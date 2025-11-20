using Krojacnica.Data;
using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class IsplateWindow : Window
    {
        private readonly otkup _otkup;
        private ObservableCollection<IsplataViewModel> isplate;
        private SnackbarMessageQueue _snackbarQueue;

        public IsplateWindow(otkup otkup)
        {
            InitializeComponent();
            _otkup = otkup;

            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
            SnackbarHost.MessageQueue = _snackbarQueue;

            isplate = new ObservableCollection<IsplataViewModel>();
            dgIsplate.ItemsSource = isplate;

            LoadIsplate();
        }

        private void LoadIsplate()
        {
            try
            {
                using var db = DbContextFactory.Create();

                var lista = db.otkup_isplata
                    .Where(oi => oi.otkup_broj_potvrde == _otkup.broj_potvrde)
                    .Select(oi => oi.isplata_broj_isplateNavigation)
                    .ToList();

                isplate.Clear();
                foreach (var i in lista)
                    isplate.Add(new IsplataViewModel(i.broj_isplate, i.datum.ToDateTime(TimeOnly.MinValue), i.iznos));

                // Prikaži ili sakrij TextBlock ako nema isplata
                txtNemaIsplata.Visibility = isplate.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                UpdateUkupno();
            }
            catch (Exception ex)
            {
                _snackbarQueue.Enqueue($"Greška pri učitavanju isplata: {ex.Message}");
            }
        }


        private void UpdateUkupno()
        {
            try
            {
                using var db = DbContextFactory.Create();

                var ukupnoIsplaceno = isplate.Sum(i => i.Iznos);
                var ukupnoVrijednostOtkupa = db.otkup_stavkas
                    .Where(s => s.otkup_broj_potvrde == _otkup.broj_potvrde)
                    .Sum(s => s.kolicina * s.materijal_dobavljac.cijena);

                txtUkupnoIsplaceno.Text = ukupnoIsplaceno.ToString("N2");
                txtPreostaloZaIsplatu.Text = (ukupnoVrijednostOtkupa - ukupnoIsplaceno).ToString("N2");
            }
            catch { }
        }

        private void DodajIsplatu_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtIznos.Text, out var iznos))
            {
                _snackbarQueue.Enqueue("Unesite ispravan iznos!");
                return;
            }

            try
            {
                using var db = DbContextFactory.Create();
                var otkupBaza = db.otkups.FirstOrDefault(o => o.broj_potvrde == _otkup.broj_potvrde);
                if (otkupBaza == null)
                {
                    _snackbarQueue.Enqueue("Otkup nije pronađen u bazi.");
                    return;
                }

                var ukupnoVrijednostOtkupa = db.otkup_stavkas
                    .Where(s => s.otkup_broj_potvrde == otkupBaza.broj_potvrde)
                    .Sum(s => s.kolicina * s.materijal_dobavljac.cijena);

                var ukupnoIsplaceno = isplate.Sum(i => i.Iznos);
                var preostaloZaIsplatu = ukupnoVrijednostOtkupa - ukupnoIsplaceno;

                if (iznos > preostaloZaIsplatu)
                {
                    _snackbarQueue.Enqueue($"Ne možete unijeti iznos veći od preostalog duga ({preostaloZaIsplatu:N2} KM).");
                    return;
                }

                // Kreiraj novu isplatu
                var isplata = new isplatum
                {
                    datum = DateOnly.FromDateTime(DateTime.Now),
                    iznos = iznos,
                    dobavljac_sifra = otkupBaza.dobavljac_sifra
                };
                db.isplata.Add(isplata);
                db.SaveChanges();

                // Veza u otkup_isplatum
                var veza = new otkup_isplatum
                {
                    otkup_broj_potvrde = otkupBaza.broj_potvrde,
                    isplata_broj_isplate = isplata.broj_isplate
                };
                db.otkup_isplata.Add(veza);
                db.SaveChanges();

                // Dodaj u ObservableCollection bez reload
                isplate.Add(new IsplataViewModel(isplata.broj_isplate, isplata.datum.ToDateTime(TimeOnly.MinValue), isplata.iznos));
                UpdateUkupno();
                txtIznos.Clear();
                txtNemaIsplata.Visibility = isplate.Count == 0 ? Visibility.Visible : Visibility.Collapsed;


                _snackbarQueue.Enqueue("Isplata uspješno dodana!");
            }
            catch (Exception ex)
            {
                _snackbarQueue.Enqueue($"Greška pri dodavanju isplate: {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fade = (Storyboard)Resources["WindowEnterAnimation"];
            fade.Begin(CardBorder);
        }
    }
}
