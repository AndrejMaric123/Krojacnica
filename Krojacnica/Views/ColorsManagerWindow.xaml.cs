using Krojacnica.Models;
using Krojacnica.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace Krojacnica.Views
{
    public partial class ColorsManagerWindow : Window
    {
        public ColorsManagerViewModel ViewModel { get; set; }
        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }

        public ColorsManagerWindow()
        {
            InitializeComponent();
            ViewModel = new ColorsManagerViewModel();
            DataContext = ViewModel;

            SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            SnackbarHost.MessageQueue = SnackbarMessageQueue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fade + slide animacija
            if (Resources["WindowEnterAnimation"] is System.Windows.Media.Animation.Storyboard sb)
                sb.Begin(CardBorder);
        }

        private void AddColor_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddOrEditColorWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                ViewModel.Boje.Add(new BojaViewModel(addWindow.Naziv, addWindow.Hex));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Button)?.DataContext is BojaViewModel selected)
            {
                BojaViewModel deletedColor = selected;

                try
                {
                    using var context = DbContextFactory.Create();
                    var existing = context.bojas.FirstOrDefault(b => b.hex_code == deletedColor.HexCode);

                    if (existing != null)
                    {
                        context.bojas.Remove(existing);
                        context.SaveChanges();

                        ViewModel.Boje.Remove(deletedColor);

                        // Snackbar samo sa Undo
                        SnackbarMessageQueue?.Enqueue(
                            $"Boja '{deletedColor.Naziv}' obrisana.",
                            "UNDO",
                            (_) =>
                            {
                                // Undo — vratimo boju u listu i u bazu
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        var undo = new boja
                                        {
                                            naziv = deletedColor.Naziv,
                                            hex_code = deletedColor.HexCode
                                        };
                                        context.bojas.Add(undo);
                                        context.SaveChanges();

                                        ViewModel.Boje.Add(deletedColor);
                                    }
                                    catch
                                    {
                                        SnackbarMessageQueue?.Enqueue(
                                            $"Ne mogu vratiti boju '{deletedColor.Naziv}'."
                                        );
                                    }
                                });
                            },
                            false
                        );
                    }
                }
                catch (Exception)
                {
                    SnackbarMessageQueue?.Enqueue(
                        $"Nije moguće obrisati boju '{deletedColor.Naziv}' jer se koristi u artiklima."
                    );
                }
            }
        }




        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
