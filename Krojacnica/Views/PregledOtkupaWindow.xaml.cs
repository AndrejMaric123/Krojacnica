using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class PregledOtkupaWindow : Window
    {
        private readonly int dobavljacSifra;

        public PregledOtkupaWindow(int sifraDobavljaca, string nazivDobavljaca)
        {
            InitializeComponent();
            dobavljacSifra = sifraDobavljaca;
            txtNaslov.Text = $"Otkupi dobavljača: {nazivDobavljaca}";
            LoadOtkupi();
        }

        private void LoadOtkupi()
        {
            using var context = DbContextFactory.Create();

            var otkupi = context.otkups
    .Include(o => o.otkup_stavkas)
        .ThenInclude(s => s.materijal_dobavljac)
            .ThenInclude(md => md.materijal)
    .Where(o => o.dobavljac_sifra == dobavljacSifra)
    .ToList();


            var podaci = otkupi.Select(o => new
            {
                Otkup = o,
                o.broj_potvrde,
                o.datum,
                Ukupno = context.otkup_stavkas
        .Where(s => s.otkup_broj_potvrde == o.broj_potvrde)
        .Sum(s => s.kolicina * s.materijal_dobavljac.cijena)
            }).ToList();


            dgOtkupi.ItemsSource = podaci;

            // Prikaz praznog teksta
            txtEmpty.Visibility = podaci.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            // izračunaj ukupne vrijednosti
            decimal ukupnoOtkup = podaci.Sum(p => p.Ukupno);
            decimal ukupnoIsplate = context.isplata
                .Where(i => i.dobavljac_sifra == dobavljacSifra)
                .Sum(i => (decimal?)i.iznos) ?? 0;

            decimal dug = ukupnoOtkup - ukupnoIsplate;

            txtUkupnoOtkup.Text = ukupnoOtkup.ToString("N2") + " KM";
            txtUkupnoIsplate.Text = ukupnoIsplate.ToString("N2") + " KM";
            txtDug.Text = dug.ToString("N2") + " KM";
        }


        private void PrikaziStavke_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int brojPotvrde)
            {
                var stavkeWin = new PregledOtkupaStavkeWindow(brojPotvrde);
                stavkeWin.ShowDialog();
            }
        }

        private void OpenIsplate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int brojPotvrde)
            {
                using var context = DbContextFactory.Create();
                var otkup = context.otkups
                    .Include(o => o.otkup_stavkas)
                    .FirstOrDefault(o => o.broj_potvrde == brojPotvrde);

                if (otkup != null)
                {
                    var window = new IsplateWindow(otkup);
                    window.ShowDialog();
                    LoadOtkupi(); // osvježi nakon isplate
                }
            }
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
