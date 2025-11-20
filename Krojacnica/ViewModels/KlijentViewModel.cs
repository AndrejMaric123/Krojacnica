public class KlijentViewModel
{
    public int Id { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Telefon { get; set; }
    public string Email { get; set; }
    public int BrojNarudzbi { get; set; }

    public string ImePrezime => $"{Ime} {Prezime}";
}
