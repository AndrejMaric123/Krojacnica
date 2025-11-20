using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Krojacnica.Views
{
    public partial class UpravljanjeZalihama : Window
    {
        private ObservableCollection<MaterijalViewModel> materijali;

        // Snackbar queue
        private SnackbarMessageQueue _snackbarQueue;


        private ICollectionView _materijaliView;

        public UpravljanjeZalihama()
        {
            InitializeComponent();

            _snackbarQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
            SnackbarHost.MessageQueue = _snackbarQueue;

            LoadMaterijali();
        }

        private void LoadMaterijali()
        {
            using var context = DbContextFactory.Create();

            var zalihe = context.materijal_zalihas
                .Include(z => z.materijal)
                .Include(z => z.boja_hex_codeNavigation)
                .Select(z => new MaterijalViewModel(z))
                .ToList();

            materijali = new ObservableCollection<MaterijalViewModel>(zalihe);

            // Napravi CollectionView za filtriranje
            _materijaliView = CollectionViewSource.GetDefaultView(materijali);
            dgMaterijali.ItemsSource = _materijaliView;

            UpdateEmptyText();
        }

        private void UpdateEmptyText()
        {
            txtEmpty.Visibility = materijali.Any() ? Visibility.Collapsed : Visibility.Visible;
        }





        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtSearch.Text.Trim().ToLower();

            if (_materijaliView == null) return;

            _materijaliView.Filter = item =>
            {
                if (item is MaterijalViewModel m)
                {
                    return string.IsNullOrWhiteSpace(query) || m.Naziv.ToLower().Contains(query);
                }
                return false;
            };

            _materijaliView.Refresh();
            UpdateEmptyText();
        }
        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void ManageDobavljaci_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaterijali.SelectedItem is MaterijalViewModel selected)
            {
                var dobavljaciWin = new MaterijalDobavljaciWindow(selected.Id);
                if (dobavljaciWin.ShowDialog() == true)
                {
                    _snackbarQueue.Enqueue($"Dobavljači materijala '{selected.Naziv}' ažurirani.");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["WindowEnterAnimation"] is Storyboard sb)
            {
                sb.Begin(CardBorder);
            }
        }
    }
}
