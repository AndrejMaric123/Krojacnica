using Krojacnica.Data;
using Krojacnica.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Krojacnica.Views
{
    public partial class EditArtikalWindow : Window
    {
        private artikal _artikal;
        private byte[]? _odabranaSlika;

        private bool _viewOnly;

        public EditArtikalWindow(artikal artikal, bool viewOnly = false)
        {
            InitializeComponent();

            _artikal = artikal;
            _viewOnly = viewOnly;

            if (_viewOnly)
            {
                btnSpremi.Visibility = Visibility.Collapsed;
            }

            LoadMaterijali();
            LoadBoje();
            LoadArtikal();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);

            txtNazivArtikla.Focus();
        }
        private void LoadMaterijali()
        {
            using var db = DbContextFactory.Create();
            cmbMaterijal.ItemsSource = db.materijals.ToList();
        }

        private void LoadBoje()
        {
            using var db = DbContextFactory.Create();
            cmbBoje.ItemsSource = db.bojas.ToList();
        }

        private void LoadArtikal()
        {

          

            txtNazivArtikla.Text = _artikal.naziv;
            txtCijenaArtikla.Text = _artikal.ponuda.jedinicna_cijena.ToString("0.00");

            // Postavi cijeli materijal objekt, ne samo naziv
            if (_artikal.materijal != null)
                cmbMaterijal.SelectedItem = ((System.Collections.IEnumerable)cmbMaterijal.ItemsSource)
                                            .Cast<materijal>()
                                            .FirstOrDefault(m => m.id == _artikal.materijal.id);

            if (_artikal.boja_hex_code != null)
                cmbBoje.SelectedItem = ((System.Collections.IEnumerable)cmbBoje.ItemsSource)
                                        .Cast<boja>()
                                        .FirstOrDefault(b => b.hex_code == _artikal.boja_hex_code);

            if (_artikal.slika != null && _artikal.slika.Length > 0)
            {
                _odabranaSlika = _artikal.slika;
                using var ms = new MemoryStream(_odabranaSlika);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                imgPreview.Source = bitmap;
            }

            if (_viewOnly)
            {
                txtNazivArtikla.IsReadOnly = true;
                txtCijenaArtikla.IsReadOnly = true;
                cmbMaterijal.IsEnabled = false;
                cmbBoje.IsEnabled = false;
                btnOdaberiSliku.IsEnabled = false;
            }

        }


        private void BtnOdaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Slike (*.jpg;*.png)|*.jpg;*.png" };
            if (dialog.ShowDialog() == true)
            {
                _odabranaSlika = File.ReadAllBytes(dialog.FileName);

                using var ms = new MemoryStream(_odabranaSlika);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                imgPreview.Source = bitmap;
            }
        }

        private void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtCijenaArtikla.Text, out var cijena))
            {
                MessageBox.Show("Unesite ispravnu cijenu!", "Greška");
                return;
            }

            if (cmbMaterijal.SelectedItem is not materijal materijal)
            {
                MessageBox.Show("Odaberite materijal!", "Greška");
                return;
            }

            if (cmbBoje.SelectedItem is not boja boja)
            {
                MessageBox.Show("Odaberite boju!", "Greška");
                return;
            }

            using var db = DbContextFactory.Create();
            var artikalDb = db.artikals.First(a => a.sifra_artikla == _artikal.sifra_artikla);
            var ponudaDb = db.ponuda.First(p => p.id == artikalDb.ponuda_id);

            artikalDb.naziv = txtNazivArtikla.Text;
            artikalDb.materijal_id = materijal.id;
            artikalDb.boja_hex_code = boja.hex_code;
            if (_odabranaSlika != null)
                artikalDb.slika = _odabranaSlika;

            ponudaDb.jedinicna_cijena = cijena;

            db.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnOtkaži_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
