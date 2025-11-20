using System.ComponentModel;
using Krojacnica.Models;

namespace Krojacnica.ViewModels
{
    public class MaterijalViewModel : INotifyPropertyChanged
    {
        private string _naziv;
        private string _kvalitet;
        private int _dostupnaKolicina;
        private string _bojaNaziv;
        private string _bojaHex;

        public string BojaNaziv
        {
            get => _bojaNaziv;
            set { _bojaNaziv = value; OnPropertyChanged(nameof(BojaNaziv)); }
        }

        public string BojaHex
        {
            get => _bojaHex;
            set { _bojaHex = value; OnPropertyChanged(nameof(BojaHex)); }
        }

        public int DostupnaKolicina
        {
            get => _dostupnaKolicina;
            set { _dostupnaKolicina = value; OnPropertyChanged(nameof(DostupnaKolicina)); }
        }


        public int Id { get; set; }

        public string Naziv
        {
            get => _naziv;
            set { _naziv = value; OnPropertyChanged(nameof(Naziv)); }
        }

        public string Kvalitet
        {
            get => _kvalitet;
            set { _kvalitet = value; OnPropertyChanged(nameof(Kvalitet)); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MaterijalViewModel() { }

        public MaterijalViewModel(materijal_zaliha zaliha)
        {
            Id = zaliha.materijal_id;
            Naziv = zaliha.materijal.naziv;
            Kvalitet = zaliha.materijal.kvalitet;
            BojaNaziv = zaliha.boja_hex_codeNavigation.naziv;
            BojaHex = zaliha.boja_hex_code;
            DostupnaKolicina = zaliha.dostupna_kolicina;
        }

    }
}
