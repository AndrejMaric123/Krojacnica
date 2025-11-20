public class ZaposleniViewModel
{
    public int OsobaId { get; set; }
    public string Ime { get; set; } = "";
    public string Prezime { get; set; } = "";
    public string KorisnickoIme { get; set; } = "";

    public DateOnly? DatumZaposlenjaOd { get; set; }
    public DateOnly? DatumZaposlenjaDo { get; set; }
}
