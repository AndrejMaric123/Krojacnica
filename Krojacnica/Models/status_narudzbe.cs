using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class status_narudzbe
{
    public string naziv { get; set; } = null!;

    public virtual ICollection<narudzba> narudzbas { get; set; } = new List<narudzba>();
}
