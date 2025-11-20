using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class racun
{
    public int broj_racuna { get; set; }

    public DateOnly datum_izdavanja { get; set; }

    public decimal ukupan_iznos { get; set; }

    public string NačinPlaćanja { get; set; } = null!;

    public int narudzba_broj_narudzbe { get; set; }

    public int klijent_osoba_id { get; set; }

    public int zaposleni_osoba_id { get; set; }

    public virtual klijent klijent_osoba { get; set; } = null!;

    public virtual narudzba narudzba_broj_narudzbeNavigation { get; set; } = null!;

    public virtual zaposleni zaposleni_osoba { get; set; } = null!;
}
