using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Krojacnica.Views
{
    public partial class PregledNarudzbeWindow : Window
    {
        private readonly PregledNarudzbeViewModel _viewModel;

        public PregledNarudzbeWindow(narudzba narudzbaEntity)
        {
            InitializeComponent();

            using var db = DbContextFactory.Create();

            var stavke = db.stavka_narudzbes
    .Include(s => s.ponuda)
        .ThenInclude(p => p.artikal)
    .Include(s => s.ponuda)
        .ThenInclude(p => p.usluga)
    .Where(s => s.narudzba_broj_narudzbe == narudzbaEntity.broj_narudzbe)
    .Select(s => new StavkaNarudzbeViewModel
    {
        PonudaId = s.ponuda_id,
        Naziv = s.ponuda.artikal != null ? s.ponuda.artikal.naziv : s.ponuda.usluga!.naziv,
        Tip = s.ponuda.artikal != null ? "Artikal" : "Usluga",
        Cijena = s.Cijena,
        Kolicina = s.kolicina,
        Ukupno = s.kolicina*s.Cijena,
        BojaHexCode = s.ponuda.artikal != null ? s.ponuda.artikal.boja_hex_code : null,
        MaterijalId = s.ponuda.artikal != null ? s.ponuda.artikal.materijal_id : 0,
        Slika = s.ponuda.artikal != null ? s.ponuda.artikal.slika : null,

        ArtikalEntity = s.ponuda.artikal,
        UslugaEntity = s.ponuda.usluga,
         StavkaEntity = s
    })
    .ToList();


            _viewModel = new PregledNarudzbeViewModel
            {
                BrojNarudzbe = narudzbaEntity.broj_narudzbe,
                Datum = narudzbaEntity.datum,
                Status = narudzbaEntity.status_narudzbe_naziv,
                Stavke = new System.Collections.ObjectModel.ObservableCollection<StavkaNarudzbeViewModel>(stavke)
            };

            DataContext = _viewModel;

            txtHeader.Text = $"Narudžba #{_viewModel.BrojNarudzbe} - {_viewModel.Status}";
            txtUkupno.Text = _viewModel.Ukupno.ToString("0.00") + " KM";

            foreach (var s in _viewModel.Stavke)
                StavkeWrap.Children.Add(CreateStavkaCard(s));
        }

        private Border CreateStavkaCard(StavkaNarudzbeViewModel vm)
        {
            // Glavni Border kartice, koristi globalni stil iz Resources
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

            // Naziv i tip
            stack.Children.Add(new TextBlock
            {
                Text = vm.Naziv,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 2),
                Foreground = (Brush)Application.Current.Resources["AppHeaderBrush"]
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Tip: {vm.Tip}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 6)
            });

            // Boja
            if (!string.IsNullOrEmpty(vm.BojaHexCode))
            {
                var colorPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
                colorPanel.Children.Add(new Border
                {
                    Width = 20,
                    Height = 20,
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom(vm.BojaHexCode),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(0, 0, 6, 0)
                });
                colorPanel.Children.Add(new TextBlock { Text = "Boja", FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
                stack.Children.Add(colorPanel);
            }

            // Količina i cijena
            stack.Children.Add(new TextBlock { Text = $"Količina: {vm.Kolicina}", FontSize = 14, Margin = new Thickness(0, 0, 0, 2) });
            stack.Children.Add(new TextBlock
            {
                Text = $"Cijena: {vm.Cijena:0.00} KM",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["AppHeaderBrush"],
                Margin = new Thickness(0, 0, 0, 2)
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"Ukupno: {vm.Ukupno:0.00} KM",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["AppHeaderBrush"],
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Dugmad
            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var btnPregledProba = new Button
            {
                Style = (Style)Application.Current.Resources["AppPrimaryButtonStyle"], // koristi stil iz ResourceDictionary
                Width = 80,
                Height = 32,
                Margin = new Thickness(4),
                Tag = vm,
                Content = new PackIcon
                {
                    Kind = PackIconKind.Eye,
                    Width = 20,
                    Height = 20,
                    Foreground = Brushes.White // bijela ikona
                }
            };
            btnPregledProba.Click += BtnPregledProba_Click;

            var btnDodajProbu = new Button
            {
                Style = (Style)Application.Current.Resources["AppPrimaryButtonStyle"], // koristi stil iz ResourceDictionary
                Width = 80,
                Height = 32,
                Margin = new Thickness(4),
                Tag = vm,
                Content = new PackIcon
                {
                    Kind = PackIconKind.Plus,
                    Width = 20,
                    Height = 20,
                    Foreground = Brushes.White // bijela ikona
                }
            };
            btnDodajProbu.Click += BtnDodajProbu_Click;

            btnPanel.Children.Add(btnPregledProba);
            btnPanel.Children.Add(btnDodajProbu);
            stack.Children.Add(btnPanel);

            return card;
        }





        private void BtnPregledProba_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is StavkaNarudzbeViewModel vm)
            {
                var probaWindow = new PregledProbaWindow(vm.StavkaEntity ?? throw new InvalidOperationException())
                { Owner = this };
                probaWindow.ShowDialog();
            }
        }

        private void BtnDodajProbu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is StavkaNarudzbeViewModel vm)
            {
                var dodajProbu = new DodajProbuWindow(vm.StavkaEntity ?? throw new InvalidOperationException())
                { Owner = this };
                dodajProbu.ShowDialog();
            }
        }

       






    }
}
