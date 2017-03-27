using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using GigaIRC.Annotations;

namespace GigaIRC.Client.WPF.Preferences
{
    public class PreferencesPage : UserControl, INotifyPropertyChanged
    {
        private Brush _designBackground;

        public Brush DesignBackground
        {
            get { return _designBackground; }
            set
            {
                if (Equals(value, _designBackground)) return;
                _designBackground = value;
                OnPropertyChanged();
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    Background = value;
                }
            }
        }

        private PreferencesTree _node;
        public PreferencesTree Node
        {
            get { return _node; }
            set
            {
                if (ReferenceEquals(value, _node)) return;
                _node = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
