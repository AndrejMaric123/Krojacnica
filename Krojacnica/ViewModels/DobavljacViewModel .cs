using Krojacnica.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Krojacnica.ViewModels
{
    public class DobavljacViewModel : INotifyPropertyChanged
    {
        private string _adresa;
        private string _telefon;
        private string _mjestoNaziv;
        private string _tip;
        private string _nazivIliIme;


       

        public int Sifra { get; set; }

        public string Adresa
        {
            get => _adresa;
            set
            {
                if (_adresa != value)
                {
                    _adresa = value;
                    OnPropertyChanged(nameof(Adresa));
                }
            }
        }

        public string Telefon
        {
            get => _telefon;
            set
            {
                if (_telefon != value)
                {
                    _telefon = value;
                    OnPropertyChanged(nameof(Telefon));
                }
            }
        }

        public string MjestoNaziv
        {
            get => _mjestoNaziv;
            set
            {
                if (_mjestoNaziv != value)
                {
                    _mjestoNaziv = value;
                    OnPropertyChanged(nameof(MjestoNaziv));
                }
            }
        }

        public string Tip
        {
            get => _tip;
            set
            {
                if (_tip != value)
                {
                    _tip = value;
                    OnPropertyChanged(nameof(Tip));
                }
            }
        }

        public string NazivIliIme
        {
            get => _nazivIliIme;
            set
            {
                if (_nazivIliIme != value)
                {
                    _nazivIliIme = value;
                    OnPropertyChanged(nameof(NazivIliIme));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DobavljacViewModel(dobavljac d)
        {
            Sifra = d.sifra;
            Adresa = d.adresa;
            Telefon = d.telefon;
            MjestoNaziv = d.mjesto_postaNavigation?.naziv ?? "";
            Tip = d.individualni != null ? "Individualni" : "Preduzeće";

            NazivIliIme = d.individualni != null
                ? $"{d.individualni.ime} {d.individualni.prezime}"
                : d.preduzece?.naziv ?? "";
        }

        public void ObrisiIzBaze()
        {
            using var context = DbContextFactory.Create();

            var db = context.dobavljacs
                .Include(d => d.individualni)
                .Include(d => d.preduzece)
                .FirstOrDefault(d => d.sifra == Sifra);

            if (db == null) return;

            if (db.individualni != null)
                context.individualnis.Remove(db.individualni);

            if (db.preduzece != null)
                context.preduzeces.Remove(db.preduzece);

            context.dobavljacs.Remove(db);

            context.SaveChanges();
        }

    }
}
