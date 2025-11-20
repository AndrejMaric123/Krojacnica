using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class admin
{
    public int zaposleni_osoba_id { get; set; }

    public virtual zaposleni zaposleni_osoba { get; set; } = null!;
}
