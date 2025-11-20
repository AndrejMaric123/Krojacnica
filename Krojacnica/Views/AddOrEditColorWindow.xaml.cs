using Krojacnica.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WinForms = System.Windows.Forms;
using MaterialDesignThemes.Wpf;

namespace Krojacnica.Views
{
    public partial class AddOrEditColorWindow : Window
    {
        private readonly boja? editingColor;
        public SnackbarMessageQueue SnackbarMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        public string Naziv { get; private set; }
        public string Hex { get; private set; }

        public AddOrEditColorWindow(boja? colorToEdit = null)
        {
            InitializeComponent();
            DataContext = this; // za Snackbar binding
            editingColor = colorToEdit;

            if (editingColor != null)
            {
                txtNaziv.Text = editingColor.naziv;
                txtHex.Text = editingColor.hex_code;

                try
                {
                    var colorObj = ColorConverter.ConvertFromString(editingColor.hex_code);
                    if (colorObj != null)
                        colorPreview.Background = new SolidColorBrush((Color)colorObj);
                }
                catch
                {
                    colorPreview.Background = Brushes.White;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)Application.Current.Resources["AppWindowEnterAnimation"];
            sb.Begin(CardBorder);
        }


        private void PickColor_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new WinForms.ColorDialog();
            if (colorDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                var color = colorDialog.Color;
                Hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                txtHex.Text = Hex;
                colorPreview.Background = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Naziv = txtNaziv.Text.Trim();
            Hex = txtHex.Text.Trim();

            if (string.IsNullOrEmpty(Naziv) || string.IsNullOrEmpty(Hex))
            {
                SnackbarMessageQueue.Enqueue("Popunite sva polja!");
                return;
            }

            using var context = DbContextFactory.Create();

            if (editingColor == null && context.bojas.Any(b => b.hex_code == Hex))
            {
                SnackbarMessageQueue.Enqueue("Boja sa ovim HEX kodom već postoji!");
                return;
            }

            if (editingColor != null)
            {
                // Izmjena postojeće boje
                editingColor.naziv = Naziv;
                editingColor.hex_code = Hex;
                context.SaveChanges();
            }
            else
            {
                // Dodavanje nove boje
                context.bojas.Add(new boja { naziv = Naziv, hex_code = Hex });
                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnPickColor_Click(object sender, RoutedEventArgs e)
        {
            colorPopup.IsOpen = true;
        }

        private void ColorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            var color = e.NewValue;
            txtHex.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            colorPreview.Background = new SolidColorBrush(color);
        }

        private void ColorPickerConfirm_Click(object sender, RoutedEventArgs e)
        {
            // colorPicker.Color je tip System.Windows.Media.Color
            Color c = colorPicker.Color;

            txtHex.Text = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            colorPreview.Background = new SolidColorBrush(c);
            colorPopup.IsOpen = false; // zatvori popup
        }







    }
}
