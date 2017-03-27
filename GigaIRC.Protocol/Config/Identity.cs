using System;
using GDDL.Structure;
using System.ComponentModel;
using GigaIRC.Annotations;
using System.Runtime.CompilerServices;
using GigaIRC.Util;

namespace GigaIRC.Config
{
    public class Identity : INotifyPropertyChanged
    {
        public SetCollection<Network> LinkedNetworks { get; } = new SetCollection<Network>();

        private string _descriptiveName;
        public string DescriptiveName
        {
            get { return _descriptiveName; }
            set
            {
                if (Equals(value, _descriptiveName)) return;
                _descriptiveName = value;
                OnPropertyChanged();
            }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (Equals(value, _fullName)) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (Equals(value, _username)) return;
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _nickname;
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                if (Equals(value, _nickname)) return;
                _nickname = value;
                OnPropertyChanged();
            }
        }

        private string _altNickname;
        public string AltNickname
        {
            get { return _altNickname; }
            set
            {
                if (Equals(value, _altNickname)) return;
                _altNickname = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Identity()
        {
        }

        public Identity(Set ident)
        {
            throw new NotImplementedException();
        }

        internal Element ToConfigString()
        {
            throw new NotImplementedException();
        }
    }
}
