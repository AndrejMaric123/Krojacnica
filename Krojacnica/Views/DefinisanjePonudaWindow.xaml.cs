using Krojacnica.Data;
using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Krojacnica.Views
{
    public partial class DefinisanjePonudaWindow : Window
    {

      
        private Dictionary<ArtikalViewModel, Border> _artikalCards = new();
        public ObservableCollection<UslugaViewModel> Usluge { get; set; } = new();
        public ObservableCollection<ArtikalViewModel> Artikli { get; set; } = new();

        private byte[]? _odabranaSlika;

        public event Action? PonudaDodana;

        public DefinisanjePonudaWindow()
        {
            InitializeComponent();

            DataContext = this;

            LoadMaterijali();
            LoadBoje();
            LoadUsluge();
            LoadArtikli();
        }

        private void LoadUsluge()
        {
            using var db = DbContextFactory.Create();
            Usluge.Clear();

            var lista = db.ponuda
                .Include(p => p.usluga)
                .Where(p => p.usluga != null)
                .ToList();

            foreach (var p in lista)
            {
                Usluge.Add(new UslugaViewModel
                {
                    Id = p.id,
                    Naziv = p.usluga!.naziv,
                    Cijena = p.jedinicna_cijena,
                    Entity = p.usluga,
                    PonudaEntity = p
                });
            }
        }

        private void LoadArtikli()
        {
            using var db = DbContextFactory.Create();
            Artikli.Clear();

            var lista = db.artikals
                .Include(a => a.materijal)
                .Include(a => a.ponuda)
                .ToList();

            foreach (var a in lista)
            {
                Artikli.Add(new ArtikalViewModel
                {
                    Sifra = a.sifra_artikla,
                    Naziv = a.naziv,
                    Cijena = a.ponuda?.jedinicna_cijena ?? 0,
                    Materijal = a.materijal?.naziv ?? "",
                    BojaHex = a.boja_hex_code ?? "",
                    Slika = a.slika,
                    Entity = a,
                    PonudaEntity = a.ponuda
                });
            }

            ArtikliWrap.Children.Clear();
            _artikalCards.Clear();
            foreach (var a in Artikli)
            {
                var card = CreateArtikalCard(a);
                ArtikliWrap.Children.Add(card);
                _artikalCards[a] = card;
            }

        }

        private void LoadMaterijali()
        {
            using var db = DbContextFactory.Create();
            cmbMaterijal.ItemsSource = db.materijals.ToList();
            cmbMaterijal.SelectedIndex = 0;
        }

        private void LoadBoje()
        {
            using var db = DbContextFactory.Create();
            cmbBoje.ItemsSource = db.bojas.ToList();
            cmbBoje.SelectedIndex = 0;
        }

        #region Usluge

        private void BtnDodajUslugu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) || !decimal.TryParse(txtCijena.Text, out var cijena))
            {
                ShowSnackbar("Unesite validan naziv i cijenu!");
                return;
            }

            using var db = DbContextFactory.Create();
            int nextId = db.ponuda.Any() ? db.ponuda.Max(p => p.id) + 1 : 1;

            var ponuda = new ponudum { id = nextId, jedinicna_cijena = cijena };
            var usluga = new usluga { naziv = txtNaziv.Text, ponuda = ponuda };

            db.ponuda.Add(ponuda);
            db.uslugas.Add(usluga);
            db.SaveChanges();

            Usluge.Add(new UslugaViewModel
            {
                Id = ponuda.id,
                Naziv = usluga.naziv,
                Cijena = cijena,
                Entity = usluga,
                PonudaEntity = ponuda
            });

            txtNaziv.Clear();
            txtCijena.Clear();
            ShowSnackbar("Usluga dodana!");
        }

        private Border CreateArtikalCard(ArtikalViewModel vm)
        {
            // Glavna kartica koristi globalni stil
            var card = new Border
            {
                Style = (Style)Application.Current.Resources["ArticleCardStyle"],
                Width = 240,
                Margin = new Thickness(10)
            };

            var stack = new StackPanel { Orientation = Orientation.Vertical };
            card.Child = stack;

            // Slika
            var img = new Image
            {
                Height = 120,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(0, 0, 0, 10),
                Clip = new RectangleGeometry(new Rect(0, 0, 240, 120), 12, 12)
            };

            if (vm.Slika != null && vm.Slika.Length > 0)
            {
                using var ms = new MemoryStream(vm.Slika);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                img.Source = bmp;
            }
            else
            {
                img.Source = new BitmapImage(new Uri("C:\\Users\\Andrej\\source\\repos\\Krojacnica\\Krojacnica\\Resources\\default_service.jpg"));
            }

            stack.Children.Add(img);

            // Naziv artikla
            stack.Children.Add(new TextBlock
            {
                Text = vm.Naziv,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 2),
                Foreground = (Brush)Application.Current.Resources["AppHeaderBrush"]
            });

            // Materijal
            stack.Children.Add(new TextBlock
            {
                Text = $"Materijal: {vm.Materijal}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 2)
            });

            // Boja
            if (!string.IsNullOrEmpty(vm.BojaHex))
            {
                var colorPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
                colorPanel.Children.Add(new Border
                {
                    Width = 20,
                    Height = 20,
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(vm.BojaHex),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(0, 0, 6, 0)
                });
                colorPanel.Children.Add(new TextBlock { Text = $"Boja: {vm.BojaNaziv}", FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
                stack.Children.Add(colorPanel);
            }

            // Cijena
            stack.Children.Add(new TextBlock
            {
                Text = $"{vm.Cijena:0.00} KM",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["AppHeaderBrush"],
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Dugmad
            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var btnEdit = new Button
            {
                Style = (Style)Application.Current.Resources["AppPrimaryButtonStyle"],
                Width = 80,
                Height = 32,
                Margin = new Thickness(4),
                Tag = vm,
                Content = new PackIcon { Kind = PackIconKind.Pencil, Width = 20, Height = 20, Foreground = Brushes.White }
            };
            btnEdit.Click += BtnUrediArtikal_Click;

            var btnDelete = new Button
            {
                Style = (Style)Application.Current.Resources["AppPrimaryButtonStyle"],
                Width = 80,
                Height = 32,
                Margin = new Thickness(4),
                Tag = vm,
                Content = new PackIcon { Kind = PackIconKind.TrashCan, Width = 20, Height = 20, Foreground = Brushes.White }
            };
            btnDelete.Click += BtnObrisiArtikal_Click;

            btnPanel.Children.Add(btnEdit);
            btnPanel.Children.Add(btnDelete);

            stack.Children.Add(btnPanel);

            return card;
        }



        private void BtnUrediUslugu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn && btn.Tag is UslugaViewModel vm)
            {
                var edit = new EditUslugaWindow(vm.PonudaEntity!, vm.Entity!) { Owner = this };
                if (edit.ShowDialog() == true)
                {
                    vm.Naziv = vm.Entity!.naziv;
                    vm.Cijena = vm.PonudaEntity!.jedinicna_cijena;
                    ShowSnackbar("Usluga ažurirana!");
                }
            }
        }

        private void BtnObrisiUslugu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn && btn.Tag is UslugaViewModel vm)
            {
                var result = MessageBox.Show($"Obrisati uslugu {vm.Naziv}?", "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                try
                {
                    using var db = DbContextFactory.Create();
                    if (vm.Entity != null) db.uslugas.Remove(vm.Entity);
                    if (vm.PonudaEntity != null) db.ponuda.Remove(vm.PonudaEntity);
                    db.SaveChanges();

                    Usluge.Remove(vm);
                    ShowSnackbar("Usluga obrisana!");
                }
                catch
                {
                    ShowSnackbar("Nije moguće obrisati uslugu!");
                }
            }
        }

        #endregion

        #region Artikli

        private void BtnOdaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Slike (*.jpg;*.png)|*.jpg;*.png" };
            if (dialog.ShowDialog() != true) return;

            _odabranaSlika = File.ReadAllBytes(dialog.FileName);

            using var ms = new MemoryStream(_odabranaSlika);
            var bmp = new System.Windows.Media.Imaging.BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            imgPreview.Source = bmp;
        }

        private void BtnDodajArtikal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazivArtikla.Text) ||
                !decimal.TryParse(txtCijenaArtikla.Text, out var cijena) ||
                cmbMaterijal.SelectedItem is not materijal mat ||
                cmbBoje.SelectedItem is not boja boja)
            {
                ShowSnackbar("Unesite validne podatke za artikal!");
                return;
            }

            using var db = DbContextFactory.Create();
            int nextSifra = db.artikals.Any() ? db.artikals.Max(a => a.sifra_artikla) + 1 : 1;
            int nextPonudaId = db.ponuda.Any() ? db.ponuda.Max(p => p.id) + 1 : 1;

            var ponuda = new ponudum { id = nextPonudaId, jedinicna_cijena = cijena };
            var artikal = new artikal
            {
                sifra_artikla = nextSifra,
                naziv = txtNazivArtikla.Text,
                ponuda = ponuda,
                materijal_id = mat.id,
                boja_hex_code = boja.hex_code,
                slika = _odabranaSlika ?? Array.Empty<byte>()
            };

            db.ponuda.Add(ponuda);
            db.artikals.Add(artikal);
            db.SaveChanges();

            var vm = new ArtikalViewModel
            {
                Sifra = artikal.sifra_artikla,
                Naziv = artikal.naziv,
                Cijena = cijena,
                Materijal = mat.naziv,
                BojaHex = boja.hex_code,
                Slika = _odabranaSlika,
                Entity = artikal,
                PonudaEntity = ponuda
            };
            Artikli.Add(vm);
            var card = CreateArtikalCard(vm);
            ArtikliWrap.Children.Add(card);
            _artikalCards[vm] = card;


            
            

            txtNazivArtikla.Clear();
            txtCijenaArtikla.Clear();
            _odabranaSlika = null;
            imgPreview.Source = null;

            ShowSnackbar("Artikal dodan!");
            PonudaDodana?.Invoke();
        }



        private void BtnObrisiArtikal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement btn || btn.Tag is not ArtikalViewModel vm)
                return;

            // odmah ukloni iz ObservableCollection i UI
            Artikli.Remove(vm);

            if (_artikalCards.TryGetValue(vm, out var card))
            {
                ArtikliWrap.Children.Remove(card);
                _artikalCards.Remove(vm);
            }

            // čuvamo reference za Undo
            var entity = vm.Entity;
            var ponudaEntity = vm.PonudaEntity;

            using var db = DbContextFactory.Create();

            try
            {
                if (entity != null) db.artikals.Remove(entity);
                if (ponudaEntity != null) db.ponuda.Remove(ponudaEntity);

                db.SaveChanges();

                PonudaDodana?.Invoke();
                // Snackbar sa undo opcijom
                MainSnackbar.MessageQueue?.Enqueue(
                    $"Artikal {vm.Naziv} obrisan",
                    "Poništi",
                    async () =>
                    {
                        using var undoDb = DbContextFactory.Create();

                        // prvo dodaj ponudu
                        if (ponudaEntity != null)
                        {
                            var ponudaCopy = new ponudum
                            {
                                id = ponudaEntity.id,
                                jedinicna_cijena = ponudaEntity.jedinicna_cijena
                            };
                            undoDb.ponuda.Add(ponudaCopy);

                            // kreiramo novu vezu za artikal
                            if (entity != null)
                            {
                                var artikalCopy = new artikal
                                {
                                    sifra_artikla = entity.sifra_artikla,
                                    naziv = entity.naziv,
                                    materijal_id = entity.materijal_id,
                                    boja_hex_code = entity.boja_hex_code,
                                    slika = entity.slika,
                                    ponuda = ponudaCopy
                                };
                                undoDb.artikals.Add(artikalCopy);

                                // update vm referenci za UI
                                vm.Entity = artikalCopy;
                                vm.PonudaEntity = ponudaCopy;
                            }
                        }

                        await undoDb.SaveChangesAsync();
                        PonudaDodana?.Invoke();

                        // Vrati u kolekciju i UI
                        Artikli.Add(vm);
                        var restoredCard = CreateArtikalCard(vm);
                        ArtikliWrap.Children.Add(restoredCard);
                        _artikalCards[vm] = restoredCard;

                        ShowSnackbar($"Brisanje artikla {vm.Naziv} poništeno!");
                    });
            }
            catch
            {
                ShowSnackbar("Nije moguće obrisati artikal!");
            }
        }




        private void BtnUrediArtikal_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn && btn.Tag is ArtikalViewModel vm)
            {
                var editWindow = new EditArtikalWindow(vm.Entity!) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    vm.Naziv = vm.Entity!.naziv;
                    vm.Cijena = vm.PonudaEntity!.jedinicna_cijena;
                    ShowSnackbar("Artikal ažuriran!");
                    PonudaDodana?.Invoke();
                }
            }
        }

        #endregion

        private void ShowSnackbar(string message) => MainSnackbar.MessageQueue?.Enqueue(message);

        private void TxtFilterUsluge_TextChanged(object sender, TextChangedEventArgs e)
        {
            using var db = DbContextFactory.Create();
            var filter = txtFilterUsluge.Text.ToLower();

            var lista = db.ponuda
                .Include(p => p.usluga)
                .Where(p => p.usluga != null && p.usluga.naziv.ToLower().Contains(filter))
                .ToList();

            Usluge.Clear();
            foreach (var p in lista)
                Usluge.Add(new UslugaViewModel
                {
                    Id = p.id,
                    Naziv = p.usluga!.naziv,
                    Cijena = p.jedinicna_cijena,
                    Entity = p.usluga,
                    PonudaEntity = p
                });
        }

        private void TxtFilterArtikli_TextChanged(object sender, TextChangedEventArgs e)
        {
            using var db = DbContextFactory.Create();
            var filter = txtFilterArtikli.Text.ToLower();

            var lista = db.artikals
                .Include(a => a.materijal)
                .Include(a => a.ponuda)
                .Where(a => a.naziv.ToLower().Contains(filter))
                .ToList();

            Artikli.Clear();
            ArtikliWrap.Children.Clear();
            foreach (var a in lista)
            {
                var vm = new ArtikalViewModel
                {
                    Sifra = a.sifra_artikla,
                    Naziv = a.naziv,
                    Cijena = a.ponuda?.jedinicna_cijena ?? 0,
                    Materijal = a.materijal?.naziv ?? "",
                    BojaHex = a.boja_hex_code ?? "",
                    Slika = a.slika,
                    Entity = a,
                    PonudaEntity = a.ponuda
                };

                Artikli.Add(vm);
                ArtikliWrap.Children.Add(CreateArtikalCard(vm));
            }
        }

    }
}
