using Krojacnica.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class UnosStavkeWindow : Window
    {
        public int Kolicina { get; private set; }
        public string HexBoja { get; private set; }

        public UnosStavkeWindow(ObservableCollection<BojaViewModel> boje)
        {
            InitializeComponent();

            // Poveži boje iz baze sa ComboBox-om
            cbBoje.ItemsSource = boje;

            // Kada korisnik izabere boju, prikazi kvadrat
            cbBoje.SelectionChanged += (s, e) =>
            {
                if (cbBoje.SelectedItem is BojaViewModel b)
                    selectedColorPreview.Background = b.PreviewBrush;
            };
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            // Validacija količine
            if (!int.TryParse(txtKolicina.Text, out int k) || k <= 0)
            {
                MessageBox.Show("Unesite validnu količinu.");
                return;
            }

            // Validacija odabrane boje
            if (cbBoje.SelectedItem is not BojaViewModel b)
            {
                MessageBox.Show("Odaberite boju.");
                return;
            }

            Kolicina = k;
            HexBoja = b.HexCode;
            DialogResult = true; // Zatvori prozor i vrati podatke
        }

        private void BtnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
