using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Krojacnica.Views
{
    public partial class EmployeesManagerWindow : Window
    {
        public ObservableCollection<ZaposleniViewModel> Employees { get; set; } = new();

        public EmployeesManagerWindow()
        {
            InitializeComponent();

            // Inicijalizacija MessageQueue
            if (SnackbarHost.MessageQueue == null)
                SnackbarHost.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

            dgEmployees.ItemsSource = Employees;
            LoadEmployees();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void LoadEmployees()
        {
            try
            {
                using var db = DbContextFactory.Create();

                Employees.Clear();

                var list = db.zaposlenis
                    .Select(z => new ZaposleniViewModel
                    {
                        OsobaId = z.osoba_id,
                        Ime = z.osoba.ime,
                        Prezime = z.osoba.prezime,
                        KorisnickoIme = z.korisnicko_ime,
                        DatumZaposlenjaOd = z.zaposlenjes.OrderByDescending(x => x.od_datuma).Select(x => x.od_datuma).FirstOrDefault(),
                        DatumZaposlenjaDo = z.zaposlenjes.OrderByDescending(x => x.do_datuma).Select(x => x.do_datuma).FirstOrDefault()
                    })
                    .OrderBy(z => z.Ime)
                    .ToList();

                foreach (var e in list)
                    Employees.Add(e);

                txtEmpty.Visibility = Employees.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                SnackbarHost.MessageQueue.Enqueue("Greška pri učitavanju zaposlenih!");
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = txtSearch.Text.Trim().ToLower();
            dgEmployees.ItemsSource = string.IsNullOrEmpty(query)
                ? Employees
                : new ObservableCollection<ZaposleniViewModel>(
                    Employees.Where(emp =>
                        emp.Ime.ToLower().Contains(query) ||
                        emp.Prezime.ToLower().Contains(query) ||
                        (emp.KorisnickoIme?.ToLower().Contains(query) ?? false)));
        }

        private async void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEmployeeWindow { Owner = this };
            if (win.ShowDialog() == true)
            {
                LoadEmployees();
                SnackbarHost.MessageQueue.Enqueue("Zaposleni uspješno dodan!");
            }
        }



        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not ZaposleniViewModel emp) return;

            try
            {
                using var db = DbContextFactory.Create();
                var z = db.zaposlenis.FirstOrDefault(x => x.osoba_id == emp.OsobaId);

                if (z != null)
                {
                    // Uklanjanje iz baze
                    db.zaposlenis.Remove(z);
                    db.SaveChanges();

                    // Uklanjanje iz ObservableCollection
                    Employees.Remove(emp);
                    txtEmpty.Visibility = Employees.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                    // Prikaz Snackbar sa Undo opcijom
                    SnackbarHost.MessageQueue.Enqueue(
                        $"Zaposleni {emp.Ime} {emp.Prezime} obrisan.",
                        "Undo",
                        () =>
                        {
                            try
                            {
                                // Vraćanje u bazu
                                using var undoDb = DbContextFactory.Create();
                                undoDb.zaposlenis.Add(z);
                                undoDb.SaveChanges();

                                // Vraćanje u ObservableCollection
                                Employees.Add(emp);
                                txtEmpty.Visibility = Visibility.Collapsed;
                            }
                            catch
                            {
                                SnackbarHost.MessageQueue.Enqueue("Greška: nije moguće vratiti zaposlenog!");
                            }
                        }
                    );
                }
            }
            catch
            {
                SnackbarHost.MessageQueue.Enqueue("Greška: nije moguće obrisati zaposlenog!");
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
