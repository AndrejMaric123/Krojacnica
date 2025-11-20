using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class DodajProbuWindow : Window
    {
        private readonly stavka_narudzbe _stavka;

        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        public DodajProbuWindow(stavka_narudzbe stavka)
        {
            InitializeComponent();
            _stavka = stavka ?? throw new ArgumentNullException(nameof(stavka));

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);

            dpDatum.Focus();
        }

        private void BtnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (dpDatum.SelectedDate == null)
            {
                SnackbarMessageQueue.Enqueue("Unesite datum probe.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtKomentar.Text))
            {
                SnackbarMessageQueue.Enqueue("Unesite komentar.");
                return;
            }

            using var db = DbContextFactory.Create();

            var novaProba = new proba
            {
                datum_probe = DateOnly.FromDateTime(dpDatum.SelectedDate.Value),
                komentar = txtKomentar.Text.Trim(),
                stavka_narudzbe_ponuda_id = _stavka.ponuda_id,
                stavka_narudzbe_narudzba_broj_narudzbe = _stavka.narudzba_broj_narudzbe
            };

            db.probas.Add(novaProba);
            db.SaveChanges();

            SnackbarMessageQueue.Enqueue("Proba je uspješno dodana.");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
