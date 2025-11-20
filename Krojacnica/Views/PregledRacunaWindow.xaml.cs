using Krojacnica.Helpers;
using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class PregledRacunaWindow : Window
    {
        private readonly RacunDetaljiViewModel _racun;

        public PregledRacunaWindow(racun racunEntity)
        {
            InitializeComponent();

            // Napuni ViewModel
            using var db = DbContextFactory.Create();

            var stavke = db.stavka_narudzbes
                .Include(s => s.ponuda)
                    .ThenInclude(p => p.artikal)
                .Include(s => s.ponuda)
                    .ThenInclude(p => p.usluga)
                .Where(s => s.narudzba_broj_narudzbe == racunEntity.narudzba_broj_narudzbe)
                .Select(s => new StavkaNarudzbeViewModel
                {
                    PonudaId = s.ponuda_id,
                    Naziv = s.ponuda.artikal != null ? s.ponuda.artikal.naziv : s.ponuda.usluga!.naziv,
                    Tip = s.ponuda.artikal != null ? "Artikal" : "Usluga",
                    Cijena = s.Cijena,
                    Kolicina = s.kolicina,
                    Ukupno = s.Cijena * s.kolicina
                })
                .ToList();

            _racun = new RacunDetaljiViewModel
            {
                BrojRacuna = racunEntity.broj_racuna,
                DatumIzdavanja = racunEntity.datum_izdavanja,
                UkupanIznos = racunEntity.ukupan_iznos,
                NacinPlacanja = racunEntity.NačinPlaćanja,
                NarudzbaBroj = racunEntity.narudzba_broj_narudzbe,
                Stavke = stavke
            };

            DataContext = _racun;
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
