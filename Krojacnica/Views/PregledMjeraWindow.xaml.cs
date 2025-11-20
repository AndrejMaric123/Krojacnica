using Krojacnica.ViewModels;
using System.Windows;

namespace Krojacnica.Views
{
    public partial class PregledMjeraWindow : Window
    {
        public PregledMjeraWindow(MjereViewModel m)
        {
            InitializeComponent();

            txtDatum.Text = m.Datum.ToString("dd.MM.yyyy");
            txtRamena.Text = m.SirinaRamena.ToString();
            txtGrudi.Text = m.ObimGrudi.ToString();
            txtStruk.Text = m.ObimStruka.ToString();
            txtKukovi.Text = m.ObimKukova.ToString();
            txtBokovi.Text = m.ObimBokova.ToString();
            txtDuzina.Text = m.Duzina.ToString();
        }



        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
