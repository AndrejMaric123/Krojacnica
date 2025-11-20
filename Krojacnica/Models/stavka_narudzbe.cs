using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class stavka_narudzbe
{
    public sbyte redni_broj { get; set; }

    public sbyte kolicina { get; set; }

    public decimal Cijena { get; set; }

    public int ponuda_id { get; set; }

    public int narudzba_broj_narudzbe { get; set; }

    public int? mjere_klijent_osoba_id { get; set; }

    public DateOnly? mjere_datum { get; set; }

    public virtual mjere? mjere { get; set; }

    public virtual narudzba narudzba_broj_narudzbeNavigation { get; set; } = null!;

    public virtual ponudum ponuda { get; set; } = null!;

    public virtual ICollection<proba> probas { get; set; } = new List<proba>();
}
