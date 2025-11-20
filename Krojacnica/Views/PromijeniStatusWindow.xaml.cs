using Krojacnica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Krojacnica.Views
{
    /// <summary>
    /// Interaction logic for PromijeniStatusWindow.xaml
    /// </summary>
    public partial class PromijeniStatusWindow : Window
    {
        private readonly narudzba _narudzba;

        public PromijeniStatusWindow(narudzba nar, List<status_narudzbe> statusi)
        {
            InitializeComponent();
            _narudzba = nar;

            cbStatus.DisplayMemberPath = "naziv"; // za prikaz u ComboBox-u
            cbStatus.ItemsSource = statusi;

            // ✅ postavi izabrani status
            cbStatus.SelectedItem = statusi.FirstOrDefault(s => s.naziv == _narudzba.status_narudzbe_naziv);
        }


        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatus.SelectedItem is not status_narudzbe noviStatus) return;

            using var db = DbContextFactory.Create();
            var n = db.narudzbas.First(x => x.broj_narudzbe == _narudzba.broj_narudzbe);
            n.status_narudzbe_naziv = noviStatus.naziv;
            db.SaveChanges();

            // ✅ update objekta koji parent window drži
            _narudzba.status_narudzbe_naziv = noviStatus.naziv;

            DialogResult = true;
            Close();
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
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
