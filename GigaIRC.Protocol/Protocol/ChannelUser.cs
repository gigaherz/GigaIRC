using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GigaIRC.Annotations;

namespace GigaIRC.Protocol
{

    public class ChannelUser : INotifyPropertyChanged, IComparable<ChannelUser>, IComparable
    {
        private readonly Connection _connection;
        private UserInfo _user;
        private string _prefix;

        public UserInfo User
        {
            get { return _user; }
            set
            {
                if (Equals(value, _user)) return;
                _user = value;
                OnPropertyChanged();
            }
        }

        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value == _prefix) return;
                _prefix = value;
                OnPropertyChanged();
            }
        }

        public ChannelUser(Connection connection, UserInfo user, string prefix)
        {
            _connection = connection;
            User = user;
            Prefix = prefix;
        }

        int IComparable.CompareTo(object obj)
        {
            var user = obj as ChannelUser;
            if (user == null)
                throw new InvalidOperationException();
            return CompareTo(user);
        }

        private static readonly int[] DefaultValue = { int.MaxValue };
        public int CompareTo(ChannelUser other)
        {
            int a = Prefix.Select(t => _connection.PreChars.IndexOf(t)).Concat(DefaultValue).Min();
            int b = other.Prefix.Select(t => _connection.PreChars.IndexOf(t)).Concat(DefaultValue).Min();

            int c = Math.Sign(a - b);
            if (c != 0)
                return c;

            return string.Compare(User.Nickname, other.User.Nickname, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Prefix + User.Nickname;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
