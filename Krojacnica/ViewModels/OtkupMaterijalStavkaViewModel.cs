using System.ComponentModel;
using Krojacnica.Models;

namespace Krojacnica.ViewModels
{
    public class OtkupMaterijalStavkaViewModel : INotifyPropertyChanged
    {
        private int _kolicina;

        public int MaterijalDobavljacId { get; set; }
        public string BojaHexCode { get; set; } = "";

        public int Kolicina
        {
            get => _kolicina;
            set
            {
                if (_kolicina != value)
                {
                    _kolicina = value;
                    OnPropertyChanged(nameof(Kolicina));
                    OnPropertyChanged(nameof(Ukupno));
                }
            }
        }

        public decimal Ukupno => Cijena * Kolicina;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _materijalNaziv = "";
        public string MaterijalNaziv
        {
            get => _materijalNaziv;
            set
            {
                if (_materijalNaziv != value)
                {
                    _materijalNaziv = value;
                    OnPropertyChanged(nameof(MaterijalNaziv));
                }
            }
        }

        private decimal _cijena;
        public decimal Cijena
        {
            get => _cijena;
            set
            {
                if (_cijena != value)
                {
                    _cijena = value;
                    OnPropertyChanged(nameof(Cijena));
                }
            }
        }

        // NOVO — kvalitet materijala
        private string _kvalitet = "";
        public string Kvalitet
        {
            get => _kvalitet;
            set
            {
                if (_kvalitet != value)
                {
                    _kvalitet = value;
                    OnPropertyChanged(nameof(Kvalitet));
                }
            }
        }

        public OtkupMaterijalStavkaViewModel(materijal_dobavljac md)
        {
            MaterijalDobavljacId = md.id;
            MaterijalNaziv = md.materijal?.naziv ?? "[Nepoznat]";
            Kvalitet = md.materijal?.kvalitet ?? "[N/A]";   // ← NOVO
            BojaHexCode = "";
            Cijena = md.cijena;
            Kolicina = 0;
        }
    }
}
