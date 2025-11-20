using Krojacnica.Models;

public class StavkaNarudzbeViewModel
{
    public int PonudaId { get; set; }
    public string Naziv { get; set; }
    public string Tip { get; set; }
    public decimal Cijena { get; set; }
    public int Kolicina { get; set; }
    public decimal Ukupno { get; set; }
    public string? BojaHexCode { get; set; }
    public int MaterijalId { get; set; }
    public byte[]? Slika { get; set; }

    // Dodaj referencu na stavku narudžbe
    public stavka_narudzbe StavkaEntity { get; set; } = null!;
    public double PotrosnjaMaterijala { get; set; }

    // Možeš dodati i referencu na entitet ako ti treba za akcije
    public artikal? ArtikalEntity { get; set; }
    public usluga? UslugaEntity { get; set; }
}