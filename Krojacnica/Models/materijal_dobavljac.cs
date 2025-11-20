using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class materijal_dobavljac
{
    public int id { get; set; }

    public int materijal_id { get; set; }

    public int dobavljac_sifra { get; set; }

    public decimal cijena { get; set; }

    public virtual dobavljac dobavljac_sifraNavigation { get; set; } = null!;

    public virtual materijal materijal { get; set; } = null!;

    public virtual ICollection<otkup_stavka> otkup_stavkas { get; set; } = new List<otkup_stavka>();
}
