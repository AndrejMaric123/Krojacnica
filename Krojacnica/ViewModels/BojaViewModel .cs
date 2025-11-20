using System.ComponentModel;
using System.Windows.Media;

namespace Krojacnica.ViewModels
{
    public class BojaViewModel : INotifyPropertyChanged
    {
        private string _naziv;
        private string _hexCode;
        private Brush _previewBrush;

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

        public string HexCode
        {
            get => _hexCode;
            set
            {
                if (_hexCode != value)
                {
                    _hexCode = value;
                    PreviewBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_hexCode));
                    OnPropertyChanged(nameof(HexCode));
                }
            }
        }

        public Brush PreviewBrush
        {
            get => _previewBrush;
            private set
            {
                _previewBrush = value;
                OnPropertyChanged(nameof(PreviewBrush));
            }
        }

        public BojaViewModel(string naziv, string hexCode)
        {
            _naziv = naziv;
            _hexCode = hexCode;
            _previewBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexCode));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
