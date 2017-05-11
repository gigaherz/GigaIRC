using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDDL.Structure;
using GigaIRC.Protocol;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GigaIRC.Annotations;
using System.Runtime.CompilerServices;

namespace GigaIRC.Config
{
    public class Network : INotifyPropertyChanged
    {
        public ObservableCollection<Server> Servers { get; } = new ObservableCollection<Server>();

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (Equals(value, _name)) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (Equals(value, _password)) return;
                _password = value;
                OnPropertyChanged();
            }
        }
        
        private Identity _defaultIdentity;
        public Identity DefaultIdentity
        {
            get => _defaultIdentity;
            set
            {
                if (ReferenceEquals(value, _defaultIdentity)) return;
                _defaultIdentity = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Network()
        {
        }

        public Network(Set net)
        {
            if (net.TryGetValue("Name", out var named))
            {
                var name = named as Value;
                if (name == null)
                    throw new InvalidDataException();

                Name = name.String;
            }

            if (net.TryGetValue("Password", out var passd))
            {
                var pass = named as Value;
                if (pass == null)
                    throw new InvalidDataException();

                Password = pass.String;
            }

            if (net.TryGetValue("DefaultIdentity", out var idd))
            {
                //var name = idd as ValueElement;
                //if (name == null)
                //    throw new InvalidDataException();

                //Name = name.ToString();
            }

            foreach (var svr in net.ByType("server"))
            {
                Servers.Add(new Server(svr));
            }
        }

        internal Element ToConfigString()
        {
            var el = new Set("network");

            el.Add(Element.StringValue(Name).WithName("Name"));
            el.Add(Element.StringValue(Password).WithName("Password"));

            if (DefaultIdentity != null)
            {
                var defIdentity = Element.StringValue(DefaultIdentity.DescriptiveName).WithName("DefaultIdentity");
                el.Add(defIdentity);
            }

            el.AddRange(Servers.Select(s => s.ToConfigString()).ToArray());

            return el;
        }

        public override string ToString()
        {
            return $"{Name}; Servers = {Servers.Count}";
        }
    }
}
