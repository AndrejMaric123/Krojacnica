using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class materijal
{
    public int id { get; set; }

    public string naziv { get; set; } = null!;

    public string kvalitet { get; set; } = null!;

    public virtual ICollection<artikal> artikals { get; set; } = new List<artikal>();

    public virtual ICollection<materijal_dobavljac> materijal_dobavljacs { get; set; } = new List<materijal_dobavljac>();

    public virtual ICollection<materijal_zaliha> materijal_zalihas { get; set; } = new List<materijal_zaliha>();
}
