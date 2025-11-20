using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class mjesto
{
    public int posta { get; set; }

    public string naziv { get; set; } = null!;

    public virtual ICollection<dobavljac> dobavljacs { get; set; } = new List<dobavljac>();
}
