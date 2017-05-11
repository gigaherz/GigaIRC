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

        public string DisplayName => Prefix + User.Nickname;

        public UserInfo User
        {
            get => _user;
            set
            {
                if (Equals(value, _user)) return;
                _user = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                if (value == _prefix) return;
                _prefix = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public ChannelUser(Connection connection, UserInfo user, string prefix)
        {
            _connection = connection;
            User = user;
            Prefix = prefix;
            user.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(user.Nickname))
                {
                    OnPropertyChanged(nameof(User));
                    OnPropertyChanged(nameof(DisplayName));
                }
            };
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
            var minThis = Prefix.Select(t => _connection.PreChars.IndexOf(t)).Concat(DefaultValue).Min();
            var minOther = other.Prefix.Select(t => _connection.PreChars.IndexOf(t)).Concat(DefaultValue).Min();

            var c = Math.Sign(minThis - minOther);
            if (c != 0)
                return c;

            return string.Compare(User.Nickname, other.User.Nickname, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
