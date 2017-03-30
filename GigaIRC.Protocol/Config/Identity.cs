using System;
using GDDL.Structure;
using System.ComponentModel;
using GigaIRC.Annotations;
using System.Runtime.CompilerServices;
using GigaIRC.Util;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

        public SetCollection<string> NicknameList { get; } = new SetCollection<string>();

        public string Nicknames
        {
            get { return string.Join(Environment.NewLine, NicknameList); }
            set
            {
                if (Equals(value, string.Join(Environment.NewLine, NicknameList))) return;
                NicknameList.Clear();
                foreach(var nn in value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    NicknameList.Add(nn);
                }
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
                if (!(named is Value name))
                    throw new InvalidDataException();

                DescriptiveName = name.String;
            }

            if (ident.TryGetValue("FullName", out var fulld))
            {
                if (!(fulld is Value fullname))
                    throw new InvalidDataException();

                FullName = fullname.String;
            }

            if (ident.TryGetValue("Username", out var userd))
            {
                if (!(userd is Value username))
                    throw new InvalidDataException();

                Username = username.String;
            }

            if (ident.TryGetValue("Nicknames", out var nickd))
            {
                if (!(nickd is Set nickList))
                    throw new InvalidDataException();

                foreach (var entry in nickList)
                {
                    if (!(entry is Value nick))
                        throw new InvalidDataException();

                    NicknameList.Add(nick.String);
                }
            }
        }

        internal Element ToConfigString()
        {
            return new Set("identity")
            {
                Element.NamedElement("DescriptiveName", Element.StringValue(DescriptiveName)),
                Element.NamedElement("FullName", Element.StringValue(FullName)),
                Element.NamedElement("Username", Element.StringValue(Username)),
                Element.NamedElement("Nicknames", Element.Set(NicknameList.Select(Element.StringValue).ToArray()))
            };
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DescriptiveName))
            {
                if (NicknameList.Count == 0)
                    return "(Unnamed)";
                return NicknameList[0];
            }
            if (NicknameList.Count > 0 && DescriptiveName != NicknameList[0])
                return string.Format("{0} ({1})", DescriptiveName, NicknameList[0]);
            return DescriptiveName;
        }
    }
}
