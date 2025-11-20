using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;

namespace Krojacnica.ViewModels
{
    public class AddMeasurementsViewModel : INotifyPropertyChanged
    {
        public SnackbarMessageQueue SnackbarMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string p) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
