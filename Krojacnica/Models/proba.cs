using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class proba
{
    public DateOnly datum_probe { get; set; }

    public int stavka_narudzbe_ponuda_id { get; set; }

    public int stavka_narudzbe_narudzba_broj_narudzbe { get; set; }

    public string komentar { get; set; } = null!;

    public virtual stavka_narudzbe stavka_narudzbe { get; set; } = null!;
}
