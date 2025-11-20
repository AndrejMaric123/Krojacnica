using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class otkup_stavka
{
    public int kolicina { get; set; }

    public int otkup_broj_potvrde { get; set; }

    public string boja_hex_code { get; set; } = null!;

    public int materijal_dobavljac_id { get; set; }

    public virtual materijal_dobavljac materijal_dobavljac { get; set; } = null!;

    public virtual otkup otkup_broj_potvrdeNavigation { get; set; } = null!;
}
