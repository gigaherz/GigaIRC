using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GigaIRC.Protocol
{
    public class ChannelModeCollection : ReadOnlyObservableCollection<ChannelMode>
    {
        public new ObservableCollection<ChannelMode> Items => (ObservableCollection<ChannelMode>)base.Items;

        internal ChannelModeCollection()
            : base(new ObservableCollection<ChannelMode>())
        {
        }

        public string this[char mchar]
        {
            get
            {
                return this.Single(u => u.Key == mchar).Value;
            }
        }

        public bool Contains(char mchar)
        {
            return this.Any(u => u.Key == mchar);
        }

        public void Remove(char mchar)
        {
            Items.Remove(Items.Single(u => u.Key == mchar));
        }

        public void Update(char mchar, string value)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == mchar)
                {
                    Items[i] = new ChannelMode(mchar, value);
                    break;
                }
            }
        }

        internal void Add(ChannelMode channelUser)
        {
            Items.Add(channelUser);
        }

        internal bool TryGetValue(char mchar, out string value)
        {
            if (Contains(mchar))
            {
                value = this[mchar];
                return true;
            }

            value = null;
            return false;
        }
    }
}