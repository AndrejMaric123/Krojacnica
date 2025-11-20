using Krojacnica.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class AddOrEditDobavljacWindow : Window
    {
        private int? editingSifra = null;
        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        public AddOrEditDobavljacWindow(int? dobavljacSifra = null)
        {
            InitializeComponent();
            LoadMjesta();

            editingSifra = dobavljacSifra;

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;
            Loaded += AddOrEditDobavljacWindow_Loaded;
        }

        private void AddOrEditDobavljacWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (editingSifra != null)
            {
                LoadDobavljacData();
                Title = "Uredi dobavljača";
                txtSifra.IsEnabled = false; // Šifra se ne mijenja kod editovanja

                // Onemogući promenu tipa kod editovanja
                rbIndividualni.IsEnabled = false;
                rbPreduzece.IsEnabled = false;
            }
            else
            {
                Title = "Dodaj dobavljača";
                txtSifra.IsEnabled = true; // Šifra se unosi za nove

                rbIndividualni.IsEnabled = true;
                rbPreduzece.IsEnabled = true;
            }

            UpdateTipVisibility();
        }


        private void LoadMjesta()
        {
            using var context = DbContextFactory.Create();
            var mjesta = context.mjestos.ToList();
            cmbMjesto.ItemsSource = mjesta;

            if (mjesta.Any()) // provjera da lista nije prazna
                cmbMjesto.SelectedIndex = 0; // izaberi prvo mjesto po defaultu
        }

        private void LoadDobavljacData()
        {
            using var context = DbContextFactory.Create();
            var d = context.dobavljacs
                .Where(x => x.sifra == editingSifra)
                .Select(x => new
                {
                    x.sifra,
                    x.adresa,
                    x.telefon,
                    x.mjesto_posta,
                    Ind = x.individualni,
                    Pred = x.preduzece
                })
                .FirstOrDefault();

            if (d == null) return;

            // Ne mijenjamo editingSifra, koristimo konstruktor
            txtSifra.Text = d.sifra.ToString();
            txtAdresa.Text = d.adresa;
            txtTelefon.Text = d.telefon;
            cmbMjesto.SelectedValue = d.mjesto_posta;

            if (d.Ind != null)
            {
                rbIndividualni.IsChecked = true;
                txtIme.Text = d.Ind.ime;
                txtPrezime.Text = d.Ind.prezime;
                txtJMB.Text = d.Ind.jmb;
            }
            else if (d.Pred != null)
            {
                rbPreduzece.IsChecked = true;
                txtNaziv.Text = d.Pred.naziv;
                txtJIB.Text = d.Pred.jib;
            }
        }

        private void Tip_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTipVisibility();
        }

        private void UpdateTipVisibility()
        {
            if (panelIndividualni == null || panelPreduzece == null) return;

            if (rbIndividualni.IsChecked == true)
            {
                panelIndividualni.Visibility = Visibility.Visible;
                panelPreduzece.Visibility = Visibility.Collapsed;
            }
            else
            {
                panelIndividualni.Visibility = Visibility.Collapsed;
                panelPreduzece.Visibility = Visibility.Visible;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMjesto.SelectedValue == null || string.IsNullOrWhiteSpace(txtAdresa.Text))
            {
                SnackbarMessageQueue?.Enqueue("Popunite sva polja.");
                return;
            }

            using var context = DbContextFactory.Create();
            dobavljac dobavljac;

            if (editingSifra != null) // EDIT
            {
                dobavljac = context.dobavljacs
                    .Include(d => d.individualni)
                    .Include(d => d.preduzece)
                    .FirstOrDefault(x => x.sifra == editingSifra);

                if (dobavljac == null)
                {
                    SnackbarMessageQueue?.Enqueue("Greška: dobavljač nije pronađen.");
                    return;
                }

                dobavljac.adresa = txtAdresa.Text;
                dobavljac.telefon = txtTelefon.Text;
                dobavljac.mjesto_posta = (int)cmbMjesto.SelectedValue;

                if (rbIndividualni.IsChecked == true)
                {
                    if (dobavljac.preduzece != null)
                        context.preduzeces.Remove(dobavljac.preduzece);

                    if (dobavljac.individualni == null)
                        dobavljac.individualni = new individualni();

                    dobavljac.individualni.ime = txtIme.Text;
                    dobavljac.individualni.prezime = txtPrezime.Text;
                    dobavljac.individualni.jmb = txtJMB.Text;
                }
                else
                {
                    if (dobavljac.individualni != null)
                        context.individualnis.Remove(dobavljac.individualni);

                    if (dobavljac.preduzece == null)
                        dobavljac.preduzece = new preduzece();

                    dobavljac.preduzece.naziv = txtNaziv.Text;
                    dobavljac.preduzece.jib = txtJIB.Text;
                }
            }
            else // ADD
            {
                if (!int.TryParse(txtSifra.Text, out int sifra))
                {
                    SnackbarMessageQueue?.Enqueue("Šifra mora biti broj.");
                    return;
                }

                dobavljac = new dobavljac
                {
                    sifra = sifra,
                    adresa = txtAdresa.Text,
                    telefon = txtTelefon.Text,
                    mjesto_posta = (int)cmbMjesto.SelectedValue
                };

                if (rbIndividualni.IsChecked == true)
                {
                    dobavljac.individualni = new individualni
                    {
                        ime = txtIme.Text,
                        prezime = txtPrezime.Text,
                        jmb = txtJMB.Text
                    };
                }
                else
                {
                    dobavljac.preduzece = new preduzece
                    {
                        naziv = txtNaziv.Text,
                        jib = txtJIB.Text
                    };
                }

                context.dobavljacs.Add(dobavljac);
            }

            context.SaveChanges();
            SnackbarMessageQueue?.Enqueue("Dobavljač uspješno sačuvan!");
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
            if (CardBorder == null)
                return; // CardBorder još nije učitan, preskoči

            if (Resources["WindowEnterAnimation"] is Storyboard sb)
                sb.Begin(CardBorder);
        }

    }
}
