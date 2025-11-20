using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class osoba
{
    public int id { get; set; }

    public string prezime { get; set; } = null!;

    public string ime { get; set; } = null!;

    public string telefon { get; set; } = null!;

    public string email { get; set; } = null!;

    public virtual klijent? klijent { get; set; }

    public virtual zaposleni? zaposleni { get; set; }
}
