using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GigaIRC.Protocol
{
    public class UserList : ReadOnlyObservableCollection<UserInfo>
    {
        public new ObservableCollection<UserInfo> Items => (ObservableCollection<UserInfo>)base.Items;

        internal UserList()
            : base(new ObservableCollection<UserInfo>())
        {
        }

        public UserInfo this[string nickname]
        {
            get
            {
                int index = IndexOfKey(nickname);
                if(index<0)
                    throw new ArgumentOutOfRangeException(nameof(nickname));
                return this[index];
            }
        }

        internal int IndexOfKey(string nickname)
        {
            for (int i = 0; i < Count; i++)
            {
                UserInfo u = this[i];
                if (u.Is(nickname))
                    return i;
            }
            return -1;
        }

        public void Remove(string nickname)
        {
            Items.Remove(Items.Single(u => u.Is(nickname)));
        }

        public void Update(UserInfo newUserInfo)
        {
            foreach (var u in this.Where(u => u.Is(newUserInfo)))
            {
                if (newUserInfo.Ident != "")
                    u.Ident = newUserInfo.Ident;
                if (newUserInfo.Host != "")
                    u.Host = newUserInfo.Host;
                return;
            }

            // If not found
            Items.Add(newUserInfo);
        }

        internal void Add(UserInfo u)
        {
            Items.Add(u);
        }
    }
}