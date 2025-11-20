public class RacunDetaljiViewModel
{
    public int BrojRacuna { get; set; }
    public DateOnly DatumIzdavanja { get; set; }
    public decimal UkupanIznos { get; set; }
    public string NacinPlacanja { get; set; } = "";
    public int NarudzbaBroj { get; set; }
    public List<StavkaNarudzbeViewModel> Stavke { get; set; } = new();
}
