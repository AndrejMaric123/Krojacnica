using Krojacnica.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Krojacnica.ViewModels
{
    public class UslugaViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _naziv = "";
        private decimal _cijena;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Naziv { get => _naziv; set { _naziv = value; OnPropertyChanged(); } }
        public decimal Cijena { get => _cijena; set { _cijena = value; OnPropertyChanged(); } }

        public usluga? Entity { get; set; }
        public ponudum? PonudaEntity { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
