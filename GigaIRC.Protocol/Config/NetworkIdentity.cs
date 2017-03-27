using GigaIRC.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GigaIRC.Config
{
    public class NetworkIdentity : INotifyPropertyChanged
    {
        private Identity _identity;
        public Identity Identity
        {
            get { return _identity; }
            set
            {
                if (ReferenceEquals(value, _identity)) return;
                _identity = value;
                OnPropertyChanged();
            }
        }

        private Network _network;
        public Network Network
        {
            get { return _network; }
            set
            {
                if (ReferenceEquals(value, _network)) return;
                _network = value;
                OnPropertyChanged();
            }
        }

        private string _saslUsername;
        public string SaslUsername
        {
            get { return _saslUsername; }
            set
            {
                if (Equals(value, _saslUsername)) return;
                _saslUsername = value;
                OnPropertyChanged();
            }
        }

        private string _saslPassword;
        public string SaslPassword
        {
            get { return _saslPassword; }
            set
            {
                if (Equals(value, _saslPassword)) return;
                _saslPassword = value;
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
