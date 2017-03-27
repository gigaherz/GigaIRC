using GigaIRC.Config;
using System;
using System.ComponentModel;
using System.Windows.Input;

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

        public RelayCommand NewNetworkCommand => new RelayCommand(_ => NewNetwork());
        private void NewNetwork()
        {
            var network = new Network();
            Settings.Networks.Add(network);
            SelectedNetwork = network;
            NetworksList.ScrollIntoView(SelectedNetwork);
            Dispatcher.BeginInvoke(new Action(() => {
                Keyboard.Focus(NetworkNameBox);
            }));
        }

        public RelayCommand RemoveNetworkCommand => new RelayCommand(_ => RemoveNetwork());
        private void RemoveNetwork()
        {
            Settings.Networks.Remove(SelectedNetwork);
        }

        public RelayCommand NewServerCommand => new RelayCommand(_ => NewServer());
        private void NewServer()
        {
            var server = new Server(SelectedNetwork);
            SelectedNetwork.Servers.Add(server);
            SelectedServer = server;
            ServersList.ScrollIntoView(SelectedServer);
            Dispatcher.BeginInvoke(new Action(() => {
                Keyboard.Focus(ServerNameBox);
            }));
        }

        public RelayCommand RemoveServerCommand => new RelayCommand(_ => RemoveServer());
        private void RemoveServer()
        {
            SelectedNetwork.Servers.Remove(SelectedServer);
        }

        public Networks()
        {
            InitializeComponent();
        }
    }
}
