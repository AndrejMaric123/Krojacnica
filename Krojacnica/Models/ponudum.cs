using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class ponudum
{
    public int id { get; set; }

    public decimal jedinicna_cijena { get; set; }

    public virtual artikal? artikal { get; set; }

    public virtual ICollection<stavka_narudzbe> stavka_narudzbes { get; set; } = new List<stavka_narudzbe>();

    public virtual usluga? usluga { get; set; }
}
