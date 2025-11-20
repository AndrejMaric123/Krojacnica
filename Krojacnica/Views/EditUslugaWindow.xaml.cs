using Krojacnica.Models;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class EditUslugaWindow : Window
    {
        private ponudum _ponuda;
        private usluga _usluga;

        public EditUslugaWindow(ponudum ponuda, usluga usluga)
        {
            InitializeComponent();
            _ponuda = ponuda;
            _usluga = usluga;

            txtNaziv.Text = usluga.naziv;
            txtCijena.Text = ponuda.jedinicna_cijena.ToString("0.00");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fade = (Storyboard)Resources["WindowEnterAnimation"];
            fade.Begin(CardBorder);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) ||
                !decimal.TryParse(txtCijena.Text, out var cijena))
            {
                SnackbarHost.MessageQueue?.Enqueue("Unesite validne podatke!");
                return;
            }

            _usluga.naziv = txtNaziv.Text;
            _ponuda.jedinicna_cijena = cijena;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
