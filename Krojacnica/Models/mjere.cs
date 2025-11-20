using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class mjere
{
    public sbyte sirina_ramena { get; set; }

    public sbyte obim_grudi { get; set; }

    public sbyte obim_struka { get; set; }

    public sbyte obim_kukova { get; set; }

    public sbyte obim_bokova { get; set; }

    public sbyte duzina { get; set; }

    public int klijent_osoba_id { get; set; }

    public DateOnly datum { get; set; }

    public virtual klijent klijent_osoba { get; set; } = null!;

    public virtual ICollection<stavka_narudzbe> stavka_narudzbes { get; set; } = new List<stavka_narudzbe>();
}
