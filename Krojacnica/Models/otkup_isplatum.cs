using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class otkup_isplatum
{
    public int id { get; set; }

    public int otkup_broj_potvrde { get; set; }

    public int isplata_broj_isplate { get; set; }

    public virtual isplatum isplata_broj_isplateNavigation { get; set; } = null!;

    public virtual otkup otkup_broj_potvrdeNavigation { get; set; } = null!;
}
