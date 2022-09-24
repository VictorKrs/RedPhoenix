using System.ComponentModel;

namespace RedPhoenix.Common.Models
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T propObj, T value, string name)
        {
            propObj = value;
            OnPropertyChanged(name);
        }

        protected void OnPropertyChanged(string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
