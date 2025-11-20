using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class materijal_zaliha
{
    public int id { get; set; }

    public int materijal_id { get; set; }

    public string boja_hex_code { get; set; } = null!;

    public int dostupna_kolicina { get; set; }

    public virtual boja boja_hex_codeNavigation { get; set; } = null!;

    public virtual materijal materijal { get; set; } = null!;
}
