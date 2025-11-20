using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class PregledOtkupaStavkeWindow : Window
    {
        public PregledOtkupaStavkeWindow(int brojPotvrde)
        {
            InitializeComponent();
            LoadStavke(brojPotvrde);
        }

        private void LoadStavke(int brojPotvrde)
        {
            using var context = DbContextFactory.Create();

            var otkup = context.otkups
                .Include(o => o.otkup_stavkas)
                    .ThenInclude(s => s.materijal_dobavljac)
                        .ThenInclude(md => md.materijal)
                .FirstOrDefault(o => o.broj_potvrde == brojPotvrde);

            if (otkup == null)
            {
                MessageBox.Show("Otkup nije pronađen.");
                Close();
                return;
            }

            txtNaslov.Text = $"Stavke otkupa #{otkup.broj_potvrde}";

            var stavke = otkup.otkup_stavkas
                .Select(s => new
                {
                    Materijal = s.materijal_dobavljac.materijal.naziv,
                    Kvalitet = s.materijal_dobavljac.materijal.kvalitet,
                    Cijena = s.materijal_dobavljac.cijena,
                    Kolicina = s.kolicina,
                    Ukupno = s.kolicina * s.materijal_dobavljac.cijena,
                    BojaHexCode = s.boja_hex_code
                })
                .ToList();

            dgStavke.ItemsSource = stavke;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
            {
                sb.Begin(CardBorder);
            }
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
