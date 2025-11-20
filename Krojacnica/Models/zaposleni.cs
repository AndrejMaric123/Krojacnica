using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class zaposleni
{
    public int osoba_id { get; set; }

    public string? korisnicko_ime { get; set; }

    public string? lozinka { get; set; }

    public virtual admin? admin { get; set; }

    public virtual ICollection<narudzba> narudzbas { get; set; } = new List<narudzba>();

    public virtual osoba osoba { get; set; } = null!;

    public virtual ICollection<racun> racuns { get; set; } = new List<racun>();

    public virtual ICollection<zaposlenje> zaposlenjes { get; set; } = new List<zaposlenje>();
}
