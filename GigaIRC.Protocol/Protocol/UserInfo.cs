using System.ComponentModel;
using System.Runtime.CompilerServices;
using GigaIRC.Annotations;

namespace GigaIRC.Protocol
{
    public class UserInfo : INotifyPropertyChanged
    {
        private string _nickname;
        private string _ident;
        private string _host;
        private string _address;

        public string Nickname
        {
            get { return _nickname; }
            internal set
            {
                if (value == _nickname) return;
                _nickname = value;
                OnPropertyChanged();
            }
        }

        public string Ident
        {
            get { return _ident; }
            internal set
            {
                if (value == _ident) return;
                _ident = value;
                OnPropertyChanged();
            }
        }

        public string Host
        {
            get { return _host; }
            internal set
            {
                if (value == _host) return;
                _host = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get { return _address; }
            internal set
            {
                if (value == _address) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public ChannelCollection SeenIn { get; private set; } = new ChannelCollection();

        internal UserInfo()
        {
            SeenIn = new ChannelCollection();
            Nickname = "";
            Ident = "";
            Host = "";
            Address = "";
        }

        internal UserInfo(string line)
        {
            int pos = line.IndexOf('@');
            if (pos >= 0)
            {
                Host = line.Substring(pos + 1);
                line = line.Substring(0, pos);
            }
            else Host = "";

            pos = line.IndexOf('!');
            if (pos >= 0)
            {
                Ident = line.Substring(pos + 1);
                line = line.Substring(0, pos);
            }
            else Ident = "";

            Address = "";

            Nickname = line;
        }

        public override string ToString()
        {
            if (Nickname == "")
                return "";

            var c = Nickname;

            if (Ident != "")
                c += "!" + Ident;

            if (Host != "")
                c += "@" + Host;

            return c;

        }

        public bool Is(string p)
        {
            return string.Compare(p, Nickname, true) == 0;
        }

        public bool Is(UserInfo u)
        {
            if (u == null)
                return false;
            return Is(u.Nickname);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
