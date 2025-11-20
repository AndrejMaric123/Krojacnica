using System;

namespace Krojacnica.ViewModels
{
    public class IsplataViewModel
    {
        public int BrojIsplate { get; set; }
        public DateTime Datum { get; set; }
        public decimal Iznos { get; set; }

        public IsplataViewModel() { }

        public IsplataViewModel(int broj, DateTime datum, decimal iznos)
        {
            BrojIsplate = broj;
            Datum = datum;
            Iznos = iznos;
        }
    }
}
