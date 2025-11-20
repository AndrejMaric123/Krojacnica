using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;

namespace Krojacnica.ViewModels
{
    public class AddEmployeeViewModel : INotifyPropertyChanged
    {
        private string _ime;
        private string _prezime;
        private string _username;
        private string _password;
        private DateTime? _odDatuma = DateTime.Now;
        private DateTime? _doDatuma;

        public string Ime
        {
            get => _ime;
            set { _ime = value; OnPropertyChanged(nameof(Ime)); }
        }

        public string Prezime
        {
            get => _prezime;
            set { _prezime = value; OnPropertyChanged(nameof(Prezime)); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public DateTime? OdDatuma
        {
            get => _odDatuma;
            set { _odDatuma = value; OnPropertyChanged(nameof(OdDatuma)); }
        }

        public DateTime? DoDatuma
        {
            get => _doDatuma;
            set { _doDatuma = value; OnPropertyChanged(nameof(DoDatuma)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string p) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        public SnackbarMessageQueue SnackbarMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
    }
}
