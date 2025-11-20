using Krojacnica.ViewModels;
using Krojacnica.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class AddMeasurementsWindow : Window
    {
        private readonly int klijentOsobaId;
        public AddMeasurementsViewModel ViewModel { get; set; }

        public AddMeasurementsWindow(int klijentOsobaId)
        {
            InitializeComponent();
            this.klijentOsobaId = klijentOsobaId;

            ViewModel = new AddMeasurementsViewModel();
            DataContext = ViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)Application.Current.Resources["AppWindowEnterAnimation"];
            sb.Begin(this);
        }


        private void SaveMeasurements_Click(object sender, RoutedEventArgs e)
        {

        
            var fields = new[]
            {
        txtRamena.Text, txtGrudi.Text, txtStruk.Text,
        txtKukovi.Text, txtBokovi.Text, txtDuzina.Text
    };


            if (fields.Any(f => string.IsNullOrWhiteSpace(f)))
            {
                ViewModel.SnackbarMessageQueue.Enqueue("Sva polja moraju biti popunjena!");
                return;
            }

            bool ContainsOnlyNumbers(string input) =>
                input.Trim().All(char.IsDigit);

            if (fields.Any(f => !ContainsOnlyNumbers(f)))
            {
                ViewModel.SnackbarMessageQueue.Enqueue("U sva polja možete unijeti samo brojeve!");
                return;
            }
            try
            {
                using var context = DbContextFactory.Create();

                sbyte TryParseMeasure(string text)
                    => sbyte.TryParse(text.Trim(), out sbyte value) ? value : (sbyte)0;

                var m = new mjere
                {
                    klijent_osoba_id = klijentOsobaId,
                    sirina_ramena = TryParseMeasure(txtRamena.Text),
                    obim_grudi = TryParseMeasure(txtGrudi.Text),
                    obim_struka = TryParseMeasure(txtStruk.Text),
                    obim_kukova = TryParseMeasure(txtKukovi.Text),
                    obim_bokova = TryParseMeasure(txtBokovi.Text),
                    duzina = TryParseMeasure(txtDuzina.Text),
                    datum = DateOnly.FromDateTime(DateTime.Now)
                };

                context.mjeres.Add(m);
                context.SaveChanges();

                ViewModel.SnackbarMessageQueue.Enqueue("Mjere uspješno sačuvane!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ViewModel.SnackbarMessageQueue.Enqueue($"Nije moguće unijeti dva puta mjere u istom danu!");
            }
        }
    }
}
