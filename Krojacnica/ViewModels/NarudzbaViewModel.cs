using System.ComponentModel;

public class NarudzbaViewModel : INotifyPropertyChanged
{
    public int BrojNarudzbe { get; set; }
    public DateOnly Datum { get; set; }

    public decimal UkupnaCijena { get; set; }

    private bool _mozeIzdatiRacun = true;
    public bool MozeIzdatiRacun
    {
        get => _mozeIzdatiRacun;
        set
        {
            _mozeIzdatiRacun = value;
            OnPropertyChanged(nameof(MozeIzdatiRacun));
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
