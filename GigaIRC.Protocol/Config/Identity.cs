using System;
using GDDL.Structure;
using System.ComponentModel;
using GigaIRC.Annotations;
using System.Runtime.CompilerServices;
using GigaIRC.Util;
using System.IO;

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
            if (ident.TryGetValue("DescriptiveName", out var named))
            {
                var name = named as Value;
                if (name == null)
                    throw new InvalidDataException();

                DescriptiveName = name.String;
            }

            if (ident.TryGetValue("FullName", out var fulld))
            {
                var fullname = fulld as Value;
                if (fullname == null)
                    throw new InvalidDataException();

                FullName = fullname.String;
            }

            if (ident.TryGetValue("Username", out var userd))
            {
                var username = userd as Value;
                if (username == null)
                    throw new InvalidDataException();

                Username = username.String;
            }

            if (ident.TryGetValue("Nickname", out var nickd))
            {
                var nick = nickd as Value;
                if (nick == null)
                    throw new InvalidDataException();

                Nickname = nick.String;
            }

            if (ident.TryGetValue("AltNickname", out var altd))
            {
                var alt = altd as Value;
                if (alt == null)
                    throw new InvalidDataException();

                AltNickname = alt.String;
            }
        }

        internal Element ToConfigString()
        {
            var el = new Set("identity");

            el.Add(Element.NamedElement("DescriptiveName", Element.StringValue(DescriptiveName)));
            el.Add(Element.NamedElement("FullName", Element.StringValue(FullName)));
            el.Add(Element.NamedElement("Username", Element.StringValue(Username)));
            el.Add(Element.NamedElement("Nickname", Element.StringValue(Nickname)));
            el.Add(Element.NamedElement("AltNickname", Element.StringValue(AltNickname)));

            return el;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DescriptiveName))
            {
                if (string.IsNullOrEmpty(Nickname))
                    return "(Unnamed)";
                return Nickname;
            }
            if (DescriptiveName != Nickname && !string.IsNullOrEmpty(Nickname))
                return string.Format("{0} ({1})", DescriptiveName, Nickname);
            return DescriptiveName;
        }
    }
}
