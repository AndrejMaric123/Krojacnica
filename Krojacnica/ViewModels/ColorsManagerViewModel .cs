using Krojacnica.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Krojacnica.ViewModels
{
    public class ColorsManagerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BojaViewModel> boje;
        private string searchQuery;

        public ObservableCollection<BojaViewModel> Boje
        {
            get => boje;
            set { boje = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => searchQuery;
            set { searchQuery = value; OnPropertyChanged(); FilterBoje(); }
        }

        public ICollectionView BojeView { get; set; }

        public ColorsManagerViewModel()
        {
            using var context = DbContextFactory.Create();
            Boje = new ObservableCollection<BojaViewModel>(
                context.bojas.Select(b => new BojaViewModel(b.naziv, b.hex_code)).ToList()
            );

            BojeView = CollectionViewSource.GetDefaultView(Boje);
        }

        private void FilterBoje()
        {
            if (BojeView == null) return;
            BojeView.Filter = string.IsNullOrWhiteSpace(SearchQuery)
                ? null
                : obj =>
                {
                    if (obj is BojaViewModel b)
                        return b.Naziv.ToLower().Contains(SearchQuery.ToLower());
                    return false;
                };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
