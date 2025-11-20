using Krojacnica.Models;

namespace Krojacnica.ViewModel
{
    public class PonudaViewModel
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Tip { get; set; } = string.Empty;
        public decimal JedinicnaCijena { get; set; }

        public artikal? Artikal { get; set; }
        public usluga? Usluga { get; set; }
    }
}
