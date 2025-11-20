using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class klijent
{
    public int osoba_id { get; set; }

    public virtual ICollection<mjere> mjeres { get; set; } = new List<mjere>();

    public virtual ICollection<narudzba> narudzbas { get; set; } = new List<narudzba>();

    public virtual osoba osoba { get; set; } = null!;

    public virtual ICollection<racun> racuns { get; set; } = new List<racun>();
}
