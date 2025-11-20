using System;
using System.Collections.Generic;

namespace Krojacnica.Models;

public partial class dobavljac
{
    public int sifra { get; set; }

    public string adresa { get; set; } = null!;

    public string telefon { get; set; } = null!;

    public int mjesto_posta { get; set; }

    public virtual individualni? individualni { get; set; }

    public virtual ICollection<isplatum> isplata { get; set; } = new List<isplatum>();

    public virtual ICollection<materijal_dobavljac> materijal_dobavljacs { get; set; } = new List<materijal_dobavljac>();

    public virtual mjesto mjesto_postaNavigation { get; set; } = null!;

    public virtual ICollection<otkup> otkups { get; set; } = new List<otkup>();

    public virtual preduzece? preduzece { get; set; }
}
