using Krojacnica.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace Krojacnica.Views
{
    public partial class AddOrEditMjestoWindow : Window
    {
        private readonly bool isEdit;
        private int? editingPosta;

        public int Posta { get; private set; }
        public string Naziv { get; private set; }

        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        public AddOrEditMjestoWindow(int? posta = null, string? naziv = null)
        {
            InitializeComponent();

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;

            if (posta.HasValue)
            {
                txtPosta.Text = posta.Value.ToString();
                txtPosta.IsEnabled = false; // pošta se ne mijenja
                txtNaziv.Text = naziv;
                isEdit = true;
                editingPosta = posta;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);

            txtPosta.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtPosta.Text.Trim(), out int posta))
            {
                SnackbarMessageQueue.Enqueue("Pošta mora biti broj!");
                return;
            }

            string naziv = txtNaziv.Text.Trim();
            if (string.IsNullOrEmpty(naziv))
            {
                SnackbarMessageQueue.Enqueue("Popunite naziv mjesta!");
                return;
            }

            using var context = DbContextFactory.Create();

            if (!isEdit)
            {
                if (context.mjestos.Any(m => m.posta == posta))
                {
                    SnackbarMessageQueue.Enqueue("Mjesto sa ovom poštom već postoji!");
                    return;
                }

                context.mjestos.Add(new mjesto { posta = posta, naziv = naziv });
            }
            else
            {
                var existing = context.mjestos.FirstOrDefault(m => m.posta == editingPosta);
                if (existing != null)
                    existing.naziv = naziv;
            }

            context.SaveChanges();

            Posta = posta;
            Naziv = naziv;

            SnackbarMessageQueue.Enqueue("Mjesto je sačuvano!");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

       
    }
}
