using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class PregledProbaWindow : Window
    {
        public PregledProbaWindow(stavka_narudzbe stavka)
        {
            InitializeComponent();

            using var db = DbContextFactory.Create();

            var probe = db.probas
                .Where(p => p.stavka_narudzbe_ponuda_id == stavka.ponuda_id &&
                            p.stavka_narudzbe_narudzba_broj_narudzbe == stavka.narudzba_broj_narudzbe)
                .OrderByDescending(p => p.datum_probe)
                .ToList();

            dgProba.ItemsSource = probe;
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
