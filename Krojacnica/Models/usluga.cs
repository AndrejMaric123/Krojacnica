using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class usluga
{
    public string naziv { get; set; } = null!;

    public int ponuda_id { get; set; }

    public virtual ponudum ponuda { get; set; } = null!;
}
