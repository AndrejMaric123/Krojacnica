using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class boja
{
    public string hex_code { get; set; } = null!;

    public string? naziv { get; set; }

    public virtual ICollection<artikal> artikals { get; set; } = new List<artikal>();

    public virtual ICollection<materijal_zaliha> materijal_zalihas { get; set; } = new List<materijal_zaliha>();
}
