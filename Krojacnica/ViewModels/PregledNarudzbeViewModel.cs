using System.Collections.ObjectModel;

public class PregledNarudzbeViewModel
{
    public int BrojNarudzbe { get; set; }
    public DateOnly Datum { get; set; }
    public string Status { get; set; } = "";
    public ObservableCollection<StavkaNarudzbeViewModel> Stavke { get; set; } = new();
    public decimal Ukupno => Stavke.Sum(s => s.Ukupno);
}
