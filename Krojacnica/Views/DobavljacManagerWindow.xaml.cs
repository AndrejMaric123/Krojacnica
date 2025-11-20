using Krojacnica.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace Krojacnica.Views
{
    public partial class DobavljacManagerWindow : Window
    {
        public DobavljacManagerViewModel ViewModel { get; set; }

        public DobavljacManagerWindow()
        {
            InitializeComponent();

            ViewModel = new DobavljacManagerViewModel();
            DataContext = ViewModel;

            // SLUŠAČ SNACKBARA – samo poruka + undo
            ViewModel.SnackbarMessage += (msg, isError, undoAction) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (undoAction != null)
                    {
                        SnackbarHost.MessageQueue?.Enqueue(
                            msg,
                            "Poništi",
                            () => undoAction()
                        );
                    }
                    else
                    {
                        SnackbarHost.MessageQueue?.Enqueue(msg);
                    }
                });
            };
        }

        private DobavljacViewModel GetRowItem(object sender)
        {
            if (sender is Button btn && btn.DataContext is DobavljacViewModel row)
                return row;
            return null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWin = new AddOrEditDobavljacWindow();
            if (addWin.ShowDialog() == true)
                ViewModel.Reload();
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetRowItem(sender);
            if (selected == null) return;

            var win = new AddOrEditDobavljacWindow(selected.Sifra);
            if (win.ShowDialog() == true)
                ViewModel.Reload();
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetRowItem(sender);
            if (selected == null) return;

            ViewModel.DeleteDobavljac(selected.Sifra);
        }

        private void PregledOtkupaRow_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetRowItem(sender);
            if (selected == null) return;

            var win = new PregledOtkupaWindow(selected.Sifra, selected.NazivIliIme);
            win.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
