using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class zaposlenje
{
    public int broj_ugovora { get; set; }

    public DateOnly od_datuma { get; set; }

    public DateOnly do_datuma { get; set; }

    public int osoba_id { get; set; }

    public virtual zaposleni osoba { get; set; } = null!;
}
