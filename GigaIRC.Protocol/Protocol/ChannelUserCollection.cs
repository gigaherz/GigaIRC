using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GigaIRC.Protocol
{
    public class ChannelUserCollection : ReadOnlyObservableCollection<ChannelUser>
    {
        public new ObservableCollection<ChannelUser> Items => (ObservableCollection<ChannelUser>)base.Items;

        internal ChannelUserCollection()
            : base(new ObservableCollection<ChannelUser>())
        {
        }

        public ChannelUser this[string nickname]
        {
            get
            {
                foreach (var u in this.Where(u => u.User.Is(nickname)))
                {
                    return u;
                }
                throw new ArgumentOutOfRangeException(nameof(nickname));
            }
        }

        public bool Contains(string nickname)
        {
            return this.Any(u => u.User.Is(nickname));
        }

        public bool Remove(string nickname)
        {
            return Items.Remove(Items.SingleOrDefault(u => u.User.Is(nickname)));
        }

        internal void Add(ChannelUser channelUser)
        {
            Items.Add(channelUser);
        }

        internal bool TryGetValue(string nickname, out ChannelUser info)
        {
            if (Contains(nickname))
            {
                info = this[nickname];
                return true;
            }

            info = null;
            return false;
        }
    }
}