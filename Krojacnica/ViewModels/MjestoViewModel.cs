using System.ComponentModel;

namespace Krojacnica.ViewModels
{
    public class MjestoViewModel : INotifyPropertyChanged
    {
        private string _naziv;

        public int Posta { get; set; }

        public string Naziv
        {
            get => _naziv;
            set
            {
                if (_naziv != value)
                {
                    _naziv = value;
                    OnPropertyChanged(nameof(Naziv));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public MjestoViewModel(int posta, string naziv)
        {
            Posta = posta;
            _naziv = naziv;
        }
    }
}
