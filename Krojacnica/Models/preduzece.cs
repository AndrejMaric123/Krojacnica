using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class preduzece
{
    public string jib { get; set; } = null!;

    public string naziv { get; set; } = null!;

    public int dobavljac_sifra { get; set; }

    public virtual dobavljac dobavljac_sifraNavigation { get; set; } = null!;
}
