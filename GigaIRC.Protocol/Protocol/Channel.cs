using System;

namespace GigaIRC.Protocol
{
    public class Channel : IComparable<Channel>, IComparable
    {
        public string Name { get; }

        public ChannelModeCollection Modes { get; } = new ChannelModeCollection();
        public ChannelUserCollection Users { get; } = new ChannelUserCollection();

        internal Channel(string name)
        {
            Name = name;
        }

        public bool Is(string channelName)
        {
            return string.Compare(Name, channelName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        int IComparable.CompareTo(object obj)
        {
            var user = obj as Channel;
            if (user == null)
                throw new InvalidOperationException();
            return CompareTo(user);
        }

        public int CompareTo(Channel other)
        {
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetDecoratedName(UserInfo userInfo)
        {
            ChannelUser info;
            if(Users.TryGetValue(userInfo.Nickname, out info))
            {
                return info.ToString();
            }

            return userInfo.Nickname;
        }
    }
}
