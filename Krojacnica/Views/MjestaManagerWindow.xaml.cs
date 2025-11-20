using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf; // za SnackbarMessageQueue
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace Krojacnica.Views
{
    public partial class MjestaManagerWindow : Window
    {
        private ObservableCollection<MjestoViewModel> mjesta;
        private SnackbarMessageQueue _snackbarQueue;

        public MjestaManagerWindow()
        {
            InitializeComponent();

            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = _snackbarQueue;

            LoadMjesta();
            dgMjesta.ItemsSource = mjesta;
        }

        private void LoadMjesta()
        {
            using var context = DbContextFactory.Create();
            mjesta = new ObservableCollection<MjestoViewModel>(
                context.mjestos.Select(m => new MjestoViewModel(m.posta, m.naziv)).ToList()
            );

            dgMjesta.ItemsSource = mjesta;
            UpdateEmptyText();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void UpdateEmptyText()
        {
            txtEmpty.Visibility = mjesta.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AddMjesto_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrEditMjestoWindow();
            if (addWindow.ShowDialog() == true)
            {
                var newMjesto = new MjestoViewModel(addWindow.Posta, addWindow.Naziv);
                mjesta.Add(newMjesto);
                UpdateEmptyText();

                _snackbarQueue.Enqueue($"Mjesto '{addWindow.Naziv}' dodano.");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dgMjesta.SelectedItem is MjestoViewModel selected)
            {
                var editWindow = new AddOrEditMjestoWindow(selected.Posta, selected.Naziv);
                if (editWindow.ShowDialog() == true)
                {
                    selected.Naziv = editWindow.Naziv;
                    _snackbarQueue.Enqueue($"Mjesto '{selected.Naziv}' ažurirano.");
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMjesta.SelectedItem is MjestoViewModel selected)
            {
                try
                {
                    using var context = DbContextFactory.Create();
                    var existing = context.mjestos.FirstOrDefault(m => m.posta == selected.Posta);

                    if (existing == null)
                    {
                        _snackbarQueue.Enqueue("Mjesto ne postoji.");
                        return;
                    }

                    // Backup za Undo
                    var backup = new MjestoViewModel(existing.posta, existing.naziv);

                    context.mjestos.Remove(existing);
                    context.SaveChanges();

                    mjesta.Remove(selected);
                    UpdateEmptyText();

                    // Snackbar sa Undo
                    _snackbarQueue.Enqueue($"Mjesto '{selected.Naziv}' obrisano.",
                        "Poništi",
                        () =>
                        {
                            using var undoContext = DbContextFactory.Create();
                            undoContext.mjestos.Add(new mjesto
                            {
                                posta = backup.Posta,
                                naziv = backup.Naziv
                            });
                            undoContext.SaveChanges();

                            mjesta.Add(backup);
                            UpdateEmptyText();
                        });
                }
                catch (DbUpdateException)
                {
                    _snackbarQueue.Enqueue($"Greška: Mjesto '{selected.Naziv}' se ne može obrisati jer je u upotrebi.");
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
            dgMjesta.ItemsSource = string.IsNullOrWhiteSpace(query)
                ? mjesta
                : new ObservableCollection<MjestoViewModel>(
                    mjesta.Where(m => m.Naziv.ToLower().Contains(query))
                  );
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
