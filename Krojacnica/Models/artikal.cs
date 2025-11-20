using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class artikal
{
    public int sifra_artikla { get; set; }

    public int ponuda_id { get; set; }

    public int materijal_id { get; set; }

    public string? boja_hex_code { get; set; }

    public byte[] slika { get; set; } = null!;

    public string naziv { get; set; } = null!;

    public virtual boja? boja_hex_codeNavigation { get; set; }

    public virtual materijal materijal { get; set; } = null!;

    public virtual ponudum ponuda { get; set; } = null!;
}
