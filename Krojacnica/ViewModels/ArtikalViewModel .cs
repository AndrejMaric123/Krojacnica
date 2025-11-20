using Krojacnica.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ArtikalViewModel : INotifyPropertyChanged
{
    private int _sifra;
    private string _naziv = "";
    private decimal _cijena;
    private string _materijal = "";
    private string _bojaHex = "";
    private string _bojaNaziv = "";
    private byte[]? _slika;

    public int Sifra { get => _sifra; set { _sifra = value; OnPropertyChanged(); } }
    public string Naziv { get => _naziv; set { _naziv = value; OnPropertyChanged(); } }
    public decimal Cijena { get => _cijena; set { _cijena = value; OnPropertyChanged(); } }
    public string Materijal { get => _materijal; set { _materijal = value; OnPropertyChanged(); } }
    public string BojaHex { get => _bojaHex; set { _bojaHex = value; OnPropertyChanged(); } }
    public string BojaNaziv { get => _bojaNaziv; set { _bojaNaziv = value; OnPropertyChanged(); } }
    public byte[]? Slika { get => _slika; set { _slika = value; OnPropertyChanged(); } }

    public artikal? Entity { get; set; }
    public ponudum? PonudaEntity { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}