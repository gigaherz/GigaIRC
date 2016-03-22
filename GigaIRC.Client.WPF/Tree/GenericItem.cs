using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GigaIRC.Client.WPF.Annotations;

namespace GigaIRC.Client.WPF.Tree
{
    public class GenericItem : INotifyPropertyChanged
    {
        private string _displayName = "";

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
