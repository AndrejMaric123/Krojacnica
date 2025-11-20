using System.ComponentModel;
using Krojacnica.Models;

namespace Krojacnica.ViewModels
{
    public class MaterijalDobavljacViewModel : INotifyPropertyChanged
    {
        private decimal _cijena;

        public int Id { get; set; }
        public int MaterijalId { get; set; }
        public int DobavljacSifra { get; set; }

        public string DobavljacNaziv { get; set; } = "";
        public string MaterijalNaziv { get; set; } = "";

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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Konstruktor iz modela EF Core
        public MaterijalDobavljacViewModel(materijal_dobavljac md)
        {
            Id = md.id;
            MaterijalId = md.materijal_id;
            DobavljacSifra = md.dobavljac_sifra;
            Cijena = md.cijena;

            DobavljacNaziv = md.dobavljac_sifraNavigation switch
            {
                { individualni: not null } => $"{md.dobavljac_sifraNavigation.individualni.ime} {md.dobavljac_sifraNavigation.individualni.prezime}",
                { preduzece: not null } => md.dobavljac_sifraNavigation.preduzece.naziv,
                _ => ""
            };

            MaterijalNaziv = md.materijal?.naziv ?? "";
        }
    }
}
