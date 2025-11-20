using Krojacnica.Models;
using Krojacnica.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace Krojacnica.Views
{
    public partial class AddOrEditMaterijalWindow : Window
    {
        private int? editingId = null;
        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        // Public property za roditelja
        public materijal Materijal { get; private set; }

        public AddOrEditMaterijalWindow(int? materijalId = null)
        {
            InitializeComponent();
            editingId = materijalId;

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;

            Loaded += AddOrEditMaterijalWindow_Loaded;
        }

        private void AddOrEditMaterijalWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (editingId != null)
            {
                LoadMaterijalData();
                Title = "Uredi materijal";
            }
            else
            {
                Title = "Dodaj materijal";
            }

            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void LoadMaterijalData()
        {
            using var context = DbContextFactory.Create();
            Materijal = context.materijals.FirstOrDefault(x => x.id == editingId);
            if (Materijal == null) return;

            txtNaziv.Text = Materijal.naziv;
            txtKvalitet.Text = Materijal.kvalitet;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) || string.IsNullOrWhiteSpace(txtKvalitet.Text))
            {
                SnackbarMessageQueue.Enqueue("Popunite naziv i kvalitet materijala.");
                return;
            }

            using var context = DbContextFactory.Create();

            if (editingId != null)
            {
                var m = context.materijals.FirstOrDefault(x => x.id == editingId);
                if (m == null)
                {
                    SnackbarMessageQueue.Enqueue("Greška: materijal nije pronađen.");
                    return;
                }
                m.naziv = txtNaziv.Text;
                m.kvalitet = txtKvalitet.Text;

                Materijal = m; // update property
            }
            else
            {
                Materijal = new materijal
                {
                    naziv = txtNaziv.Text,
                    kvalitet = txtKvalitet.Text,
                };
                context.materijals.Add(Materijal);
            }

            context.SaveChanges();
            SnackbarMessageQueue.Enqueue("Materijal je sačuvan!");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);
        }
    }
}
