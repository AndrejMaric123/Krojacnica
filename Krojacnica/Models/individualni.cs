using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class individualni
{
    public string jmb { get; set; } = null!;

    public string prezime { get; set; } = null!;

    public string ime { get; set; } = null!;

    public int dobavljac_sifra { get; set; }

    public virtual dobavljac dobavljac_sifraNavigation { get; set; } = null!;
}
