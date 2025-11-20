using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Krojacnica.ViewModels
{
    public class MaterijalManagerViewModel : INotifyPropertyChanged
    {
        private int id;
        private string naziv = string.Empty;
        private string kvalitet = string.Empty;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        public string Naziv
        {
            get => naziv;
            set { naziv = value; OnPropertyChanged(); }
        }

        public string Kvalitet
        {
            get => kvalitet;
            set { kvalitet = value; OnPropertyChanged(); }
        }

        public MaterijalManagerViewModel() { }

        public MaterijalManagerViewModel(Krojacnica.Models.materijal m)
        {
            Id = m.id;
            Naziv = m.naziv;
            Kvalitet = m.kvalitet;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
