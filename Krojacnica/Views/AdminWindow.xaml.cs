using Krojacnica.Data;
using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Krojacnica.Views
{
    
        public partial class AdminWindow : Window
        {
            private readonly zaposleni currentAdmin;
            public ObservableCollection<ArtikalViewModel> Artikli { get; set; } = new();
            private byte[]? _odabranaSlikaAdmin;

            public AdminWindow(zaposleni admin)
            {
                InitializeComponent();
                currentAdmin = admin;

                string savedTheme = Properties.Settings.Default.AppTheme;
                if (!string.IsNullOrEmpty(savedTheme))
                    ChangeTheme(savedTheme);

                string savedLang = Properties.Settings.Default.AppLanguage;
                if (!string.IsNullOrEmpty(savedLang))
                    ChangeLanguage(savedLang);

                LoadArtikli();
            }

   

            private void LoadArtikli()
            {
                Artikli.Clear();
                MainCardsWrapPanel.Children.Clear();

                using var db = DbContextFactory.Create();
                var lista = db.artikals
                    .Include(a => a.materijal)
                    .Include(a => a.ponuda)
                    .ToList();

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
                    MainCardsWrapPanel.Children.Add(CreateArtikalCard(vm));
                }
            }

            private Border CreateArtikalCard(ArtikalViewModel vm)
            {
                var card = new Border
                {
                    Width = 240,
                    Margin = new Thickness(10)
                };
                card.SetResourceReference(Border.StyleProperty, "ArticleCardStyle");

                var stack = new StackPanel { Orientation = Orientation.Vertical };
                card.Child = stack;

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
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/default_service.jpg"));
                }
                stack.Children.Add(img);

                var nazivText = new TextBlock
                {
                    Text = vm.Naziv,
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 2)
                };
                nazivText.SetResourceReference(TextBlock.ForegroundProperty, "AppHeaderBrush");
                stack.Children.Add(nazivText);

                var materijalText = new TextBlock
                {
                    Text = $"Materijal: {vm.Materijal}",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 2)
                };
                materijalText.SetResourceReference(TextBlock.ForegroundProperty, "AppTextBoxHintBrush");
                stack.Children.Add(materijalText);

                if (!string.IsNullOrEmpty(vm.BojaHex))
                {
                    var colorPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
                    var bojaText = new TextBlock
                    {
                        Text = $"Boja: {vm.BojaNaziv}",
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    bojaText.SetResourceReference(TextBlock.ForegroundProperty, "AppTextBoxHintBrush");
                    colorPanel.Children.Add(bojaText);
                    colorPanel.Children.Add(new Border
                    {
                        Width = 20,
                        Height = 20,
                        Background = (Brush)new BrushConverter().ConvertFrom(vm.BojaHex),
                        CornerRadius = new CornerRadius(4),
                        Margin = new Thickness(0, 0, 6, 0)
                    });
                    stack.Children.Add(colorPanel);
                }

                var cijenaText = new TextBlock
                {
                    Text = $"{vm.Cijena:0.00} KM",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                cijenaText.SetResourceReference(TextBlock.ForegroundProperty, "AppHeaderBrush");
                stack.Children.Add(cijenaText);

            

                return card;
            }

        

      





        private void ViewEmployees_Click(object sender, RoutedEventArgs e)
        {
            
            OpenChildWindow(new EmployeesManagerWindow());
        }

        private void ViewClients_Click(object sender, RoutedEventArgs e)
        {
         
            OpenChildWindow(new PregledKlijenataWindow());
        }

        private void ManageColors_Click(object sender, RoutedEventArgs e)
        {
    
            OpenChildWindow(new ColorsManagerWindow());
        }

        private void ManagePlaces_Click(object sender, RoutedEventArgs e)
        {
         
            OpenChildWindow(new MjestaManagerWindow());
        }

        private void ManageDobavljaci_Click(object sender, RoutedEventArgs e)
        {
         
            OpenChildWindow(new DobavljacManagerWindow());
        }

        private void ManageMaterijali_Click(object sender, RoutedEventArgs e)
        {
            ;
            OpenChildWindow(new MaterijalManagerWindow());
        }

        private void ManageZalihe_Click(object sender, RoutedEventArgs e)
        {
      
            OpenChildWindow(new UpravljanjeZalihama());
        }

        private void ManageOtkupi_Click(object sender, RoutedEventArgs e)
        {
          
            OpenChildWindow(new OtkupMaterijalaWindow());
        }

        private void ManagePonude_Click(object sender, RoutedEventArgs e)
        {
            var ponudaWindow = new DefinisanjePonudaWindow();
            ponudaWindow.PonudaDodana += () =>
            {
                // Refresh artikala i ponuda u AdminWindow
                LoadArtikli();
            };
            OpenChildWindow(ponudaWindow);
        }


        private void ManageDodajNarudbzu_Click(object sender, RoutedEventArgs e)
        {
            
            OpenChildWindow(new NovaNarudzbaWindow());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LoggedUserId = 0;
            Properties.Settings.Default.LoggedUserRole = string.Empty;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ChangeTheme(string themeName)
        {
      
            var uri = new Uri($"/Resources/{themeName}.xaml", UriKind.Relative);

            
            var existingDictionaries = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("theme")).ToList();

            foreach (var dict in existingDictionaries)
                Application.Current.Resources.MergedDictionaries.Remove(dict);

           
            var newDict = new ResourceDictionary() { Source = uri };
            Application.Current.Resources.MergedDictionaries.Add(newDict);

         
            Properties.Settings.Default.AppTheme = themeName;
            Properties.Settings.Default.Save();
        }

        private void BtnLightTheme_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme("LightTheme");
        }

        private void BtnDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme("DarkTheme");
        }

        private void BtnGrayTheme_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme("GrayTheme");
        }

        

        private void ChangeLanguage(string langCode)
        {
            string fileName = $"StringResources.{langCode}.xaml";
            var uri = new Uri($"/Resources/{fileName}", UriKind.Relative);

       
            var oldLangDicts = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("StringResources"))
                .ToList();

            foreach (var dict in oldLangDicts)
                Application.Current.Resources.MergedDictionaries.Remove(dict);

            var newDict = new ResourceDictionary() { Source = uri };
            Application.Current.Resources.MergedDictionaries.Add(newDict);

          
            Properties.Settings.Default.AppLanguage = langCode;
            Properties.Settings.Default.Save();
        }


        private void BtnLangBS_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("bs");
        }

        private void BtnLangEN_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("en");
        }

        private void OpenChildWindow(Window child)
        {
            // Onemogući parent, ali ga ne sakrivaj
            this.IsEnabled = false;

            child.Owner = this;

            // Kada se child zatvori, ponovo aktiviraj parent
            child.Closed += (s, e) => this.IsEnabled = true;

            child.ShowDialog();
        }





    }
}
