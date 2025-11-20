using Krojacnica.Models;
using Krojacnica.ViewModels;
using Microsoft.EntityFrameworkCore;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Krojacnica.Views
{
    public partial class MaterijalDobavljaciWindow : Window
    {
        private readonly int _materijalId;
        private ObservableCollection<MaterijalDobavljacViewModel> postojeciVeze;
        private SnackbarMessageQueue _snackbarQueue;

        public MaterijalDobavljaciWindow(int materijalId)
        {
            InitializeComponent();
            _materijalId = materijalId;

            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = _snackbarQueue;

            Loaded += MaterijalDobavljaciWindow_Loaded;
        }

        private void MaterijalDobavljaciWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            LoadDobavljaci();
            LoadPostojeciVeze();
        }

        private void LoadDobavljaci()
        {
            using var context = DbContextFactory.Create();
            var dobavljaci = context.dobavljacs
                .Include(d => d.individualni)
                .Include(d => d.preduzece)
                .Include(d => d.mjesto_postaNavigation)
                .Select(d => new DobavljacViewModel(d))
                .ToList();

            dgDobavljaci.ItemsSource = dobavljaci;
        }

        private void LoadPostojeciVeze()
        {
            using var context = DbContextFactory.Create();
            postojeciVeze = new ObservableCollection<MaterijalDobavljacViewModel>(
                context.materijal_dobavljacs
                    .Where(md => md.materijal_id == _materijalId)
                    .Include(md => md.dobavljac_sifraNavigation)
                        .ThenInclude(d => d.individualni)
                    .Include(md => md.dobavljac_sifraNavigation)
                        .ThenInclude(d => d.preduzece)
                    .Include(md => md.dobavljac_sifraNavigation)
                        .ThenInclude(d => d.mjesto_postaNavigation)
                    .Include(md => md.materijal)
                    .Select(md => new MaterijalDobavljacViewModel(md))
                    .ToList()
            );

            dgPostojeciVeze.ItemsSource = postojeciVeze;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (dgDobavljaci.SelectedItem is not DobavljacViewModel selectedDobavljac)
            {
                _snackbarQueue.Enqueue("Odaberite dobavljača.");
                return;
            }

            if (!decimal.TryParse(txtCijena.Text, out decimal cijena))
            {
                _snackbarQueue.Enqueue("Unesite ispravnu cijenu.");
                return;
            }

            try
            {
                using var context = DbContextFactory.Create();

                var existing = context.materijal_dobavljacs
                    .FirstOrDefault(md => md.materijal_id == _materijalId && md.dobavljac_sifra == selectedDobavljac.Sifra);

                if (existing != null)
                {
                    existing.cijena = cijena;
                    _snackbarQueue.Enqueue($"Cijena ažurirana za '{selectedDobavljac.Sifra}'.");
                }
                else
                {
                    var novi = new materijal_dobavljac
                    {
                        materijal_id = _materijalId,
                        dobavljac_sifra = selectedDobavljac.Sifra,
                        cijena = cijena
                    };
                    context.materijal_dobavljacs.Add(novi);
                    _snackbarQueue.Enqueue($"Veza sa '{selectedDobavljac.Sifra}' dodana.");
                }

                context.SaveChanges();
                LoadPostojeciVeze();
            }
            catch (Exception ex)
            {
                _snackbarQueue.Enqueue($"Greška pri čuvanju: {ex.Message}");
            }
        }

        private void DeleteVeza_Click(object sender, RoutedEventArgs e)
        {
            if (dgPostojeciVeze.SelectedItem is not MaterijalDobavljacViewModel selected)
                return;

            try
            {
                using var context = DbContextFactory.Create();
                var veza = context.materijal_dobavljacs.FirstOrDefault(md => md.id == selected.Id);
                if (veza != null)
                {
                    context.materijal_dobavljacs.Remove(veza);
                    context.SaveChanges();
                    postojeciVeze.Remove(selected);

                    // Snackbar sa undo opcijom
                    _snackbarQueue.Enqueue(
                        $"Veza sa '{selected.DobavljacNaziv}' obrisana.",
                        "UNDO",
                        () =>
                        {
                            using var undoContext = DbContextFactory.Create();
                            undoContext.materijal_dobavljacs.Add(veza);
                            undoContext.SaveChanges();
                            postojeciVeze.Add(selected);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                _snackbarQueue.Enqueue($"Greška pri brisanju: {ex.Message}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
