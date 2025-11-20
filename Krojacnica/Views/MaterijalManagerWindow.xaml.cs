using Krojacnica.Models;
using Krojacnica.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf; // <--- Za SnackbarMessageQueue

namespace Krojacnica.Views
{
    public partial class MaterijalManagerWindow : Window
    {
        private ObservableCollection<MaterijalManagerViewModel> materijali = new();

        // Snackbar queue
        private SnackbarMessageQueue _snackbarQueue;

        public MaterijalManagerWindow()
        {
            InitializeComponent();

            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = _snackbarQueue;

            LoadMaterijali();
            dgMaterijali.ItemsSource = materijali; // jednom postavi ItemsSource
        }

        private void LoadMaterijali()
        {
            using var context = DbContextFactory.Create();

            var data = context.materijals
                              .Select(m => new MaterijalManagerViewModel(m))
                              .ToList();

            materijali.Clear();
            foreach (var m in data)
                materijali.Add(m);

            UpdateEmptyText();
        }

        private void UpdateEmptyText()
        {
            txtEmpty.Visibility = materijali.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWin = new AddOrEditMaterijalWindow();
            if (addWin.ShowDialog() == true)
            {
                if (addWin.Materijal != null)
                {
                    materijali.Add(new MaterijalManagerViewModel(addWin.Materijal));
                    UpdateEmptyText();
                    _snackbarQueue.Enqueue($"Materijal '{addWin.Materijal.naziv}' dodan.");
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaterijali.SelectedItem is MaterijalManagerViewModel selected)
            {
                var editWin = new AddOrEditMaterijalWindow(selected.Id);
                if (editWin.ShowDialog() == true)
                {
                    selected.Naziv = editWin.Materijal.naziv;
                    selected.Kvalitet = editWin.Materijal.kvalitet;
                    _snackbarQueue.Enqueue($"Materijal '{selected.Naziv}' je ažuriran.");
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaterijali.SelectedItem is MaterijalManagerViewModel selected)
            {
                // Pokušaj obrisati
                try
                {
                    using var context = DbContextFactory.Create();
                    var existing = context.materijals.FirstOrDefault(m => m.id == selected.Id);

                    if (existing == null)
                    {
                        _snackbarQueue.Enqueue("Materijal ne postoji.");
                        return;
                    }

                    // Sačuvaj za undo
                    var backup = new MaterijalManagerViewModel(existing);

                    context.materijals.Remove(existing);
                    context.SaveChanges();

                    materijali.Remove(selected);
                    UpdateEmptyText();

                    // Snackbar sa Undo
                    _snackbarQueue.Enqueue($"Materijal '{selected.Naziv}' obrisan.",
                        "Poništi",
                        () =>
                        {
                            using var undoContext = DbContextFactory.Create();
                            undoContext.materijals.Add(new materijal
                            {
                                id = backup.Id,
                                naziv = backup.Naziv,
                                kvalitet = backup.Kvalitet
                            });
                            undoContext.SaveChanges();

                            materijali.Add(backup);
                            UpdateEmptyText();
                        });
                }
                catch (DbUpdateException)
                {
                    // Npr. FK constraint -> materijal u upotrebi
                    _snackbarQueue.Enqueue($"Greška: Materijal '{selected.Naziv}' se ne može obrisati jer je u upotrebi.");
                }
                catch (Exception ex)
                {
                    _snackbarQueue.Enqueue($"Neočekivana greška: {ex.Message}");
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtSearch.Text.ToLower();
            dgMaterijali.ItemsSource = string.IsNullOrWhiteSpace(query)
                ? materijali
                : new ObservableCollection<MaterijalManagerViewModel>(
                    materijali.Where(m => m.Naziv.ToLower().Contains(query))
                  );
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
