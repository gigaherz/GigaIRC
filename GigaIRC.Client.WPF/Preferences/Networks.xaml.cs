using GigaIRC.Config;
using System.ComponentModel;

namespace GigaIRC.Client.WPF.Preferences
{
    public partial class Networks : INotifyPropertyChanged
    {
        public Settings Settings => MainWindow.Instance.Session.Settings;
        
        private Network _selectedNetwork;
        public Network SelectedNetwork
        {
            get { return _selectedNetwork; }
            set
            {
                if (ReferenceEquals(value, _selectedNetwork)) return;
                _selectedNetwork = value;
                OnPropertyChanged();
            }
        }

        private Server _selectedServer;
        public Server SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                if (ReferenceEquals(value, _selectedServer)) return;
                _selectedServer = value;
                OnPropertyChanged();
            }
        }

        public Networks()
        {
            InitializeComponent();
        }
    }
}
