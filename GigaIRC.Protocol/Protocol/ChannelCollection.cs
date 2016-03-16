using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GigaIRC.Protocol
{
    public class ChannelCollection : ReadOnlyObservableCollection<Channel>
    {
        public new ObservableCollection<Channel> Items => (ObservableCollection<Channel>)base.Items;

        internal ChannelCollection()
            : base(new ObservableCollection<Channel>())
        {
        }

        public Channel this[string channelName]
        {
            get
            {
                foreach (var c in this.Where(c => c.Is(channelName)))
                {
                    return c;
                }
                throw new ArgumentOutOfRangeException(nameof(channelName));
            }
        }

        public bool Contains(string channelName)
        {
            return this.Any(u => u.Is(channelName));
        }

        internal bool Remove(string channelName)
        {
            return Items.Remove(Items.SingleOrDefault(u => u.Is(channelName)));
        }

        internal void Add(Channel ch)
        {
            Items.Add(ch);
        }

        internal void Clear()
        {
            Items.Clear();
        }
    }
}