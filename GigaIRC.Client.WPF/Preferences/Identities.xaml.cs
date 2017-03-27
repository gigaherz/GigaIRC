using GigaIRC.Config;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace GigaIRC.Client.WPF.Preferences
{
    public partial class Identities : INotifyPropertyChanged
    {
        public Settings Settings => MainWindow.Instance.Session.Settings;

        private Identity _selectedIdentity;
        public Identity SelectedIdentity
        {
            get { return _selectedIdentity; }
            set
            {
                if (ReferenceEquals(value, _selectedIdentity)) return;
                _selectedIdentity = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NewIdentityCommand => new RelayCommand(_ => NewIdentity());
        private void NewIdentity()
        {
            var identity = new Identity();
            Settings.Identities.Add(identity);
            SelectedIdentity = identity;
            IdentitiesList.ScrollIntoView(SelectedIdentity);
            Dispatcher.BeginInvoke(new Action(() => {
                Keyboard.Focus(NameBox);
            }));
        }

        public RelayCommand RemoveIdentityCommand => new RelayCommand(_ => RemoveIdentity());
        private void RemoveIdentity()
        {
            Settings.Identities.Remove(SelectedIdentity);
        }

        public Identities()
        {
            InitializeComponent();
        }
    }
}
