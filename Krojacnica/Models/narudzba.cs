using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class narudzba
{
    public int broj_narudzbe { get; set; }

    public DateOnly datum { get; set; }

    public DateOnly? datum_otkazivanja { get; set; }

    public int zaposleni_osoba_id { get; set; }

    public int klijent_osoba_id { get; set; }

    public string status_narudzbe_naziv { get; set; } = null!;

    public virtual klijent klijent_osoba { get; set; } = null!;

    public virtual ICollection<racun> racuns { get; set; } = new List<racun>();

    public virtual status_narudzbe status_narudzbe_nazivNavigation { get; set; } = null!;

    public virtual ICollection<stavka_narudzbe> stavka_narudzbes { get; set; } = new List<stavka_narudzbe>();

    public virtual zaposleni zaposleni_osoba { get; set; } = null!;
}
