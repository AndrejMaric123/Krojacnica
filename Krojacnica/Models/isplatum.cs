using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class isplatum
{
    public int broj_isplate { get; set; }

    public DateOnly datum { get; set; }

    public decimal iznos { get; set; }

    public int dobavljac_sifra { get; set; }

    public virtual dobavljac dobavljac_sifraNavigation { get; set; } = null!;

    public virtual ICollection<otkup_isplatum> otkup_isplata { get; set; } = new List<otkup_isplatum>();
}
