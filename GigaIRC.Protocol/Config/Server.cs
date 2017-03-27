using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDDL.Structure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GigaIRC.Annotations;
using System.Runtime.CompilerServices;

namespace GigaIRC.Config
{
    public class Server : INotifyPropertyChanged
    {
        public Network Network { get; }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (Equals(value, _displayName)) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (Equals(value, _address)) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<int, int>> PortRangeCollection { get; } = new ObservableCollection<Tuple<int, int>>();

        public ObservableCollection<Tuple<int, int>> SecurePortRangeCollection { get; } = new ObservableCollection<Tuple<int, int>>();

        public string PortRanges
        {
            get { return SerializeRanges(PortRangeCollection); }
            set
            {
                if (Equals(value, SerializeRanges(PortRangeCollection))) return;
                ParsePortRanges(PortRangeCollection, value);
                OnPropertyChanged();
            }
        }

        public string SecurePortRanges
        {
            get { return SerializeRanges(SecurePortRangeCollection); }
            set
            {
                if (Equals(value, SerializeRanges(SecurePortRangeCollection))) return;
                ParsePortRanges(SecurePortRangeCollection, value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Server(Network n)
        {
            Network = n;
        }

        public Server(Set svr)
        {
            Element named;

            if (svr.TryGetValue("DisplayName", out named))
            {
                var name = named as Value;
                if (name == null)
                    throw new InvalidDataException();

                DisplayName = name.String;
            }

            if (svr.TryGetValue("Address", out named))
            {
                var addr = named as Value;
                if (addr == null)
                    throw new InvalidDataException();

                Address = addr.String;
            }

            if (svr.TryGetValue("PortRanges", out named))
            {
                var ports = named as Set;
                if (ports == null)
                    throw new InvalidDataException();

                LoadRanges(PortRangeCollection, ports);
            }

            if (svr.TryGetValue("SecurePortRanges", out named))
            {
                var ports = named as Set;
                if (ports == null)
                    throw new InvalidDataException();

                LoadRanges(SecurePortRangeCollection, ports);
            }

        }

        private static void LoadRanges(ICollection<Tuple<int, int>> portRanges, IEnumerable<Element> ports)
        {
            foreach (var el in ports)
            {
                var vel = (el as Value);
                var sel = (el as Set);

                if(vel != null)
                {
                    var port1 = (int)vel.AsValue().Integer;
                    portRanges.Add(new Tuple<int, int>(port1, port1));
                }
                else if(sel != null)
                {
                    var el1 = sel[0] as Value;
                    var el2 = sel[1] as Value;
                    if (el1 != null && el2 != null)
                    {
                        var port1 = (int)el1.AsValue().Integer;
                        var port2 = (int)el2.AsValue().Integer;
                        portRanges.Add(new Tuple<int, int>(port1, port2));
                    }
                }
                else
                    throw new InvalidDataException();
            }
        }

        internal Element ToConfigString()
        {
            var el = new Set("server");

            var name = Element.NamedElement("DisplayName", Element.StringValue(DisplayName));
            el.Add(name);

            var addr = Element.NamedElement("Address", Element.StringValue(Address));
            el.Add(addr);

            if (PortRangeCollection.Count > 0)
            {
                var ranges = Element.NamedElement("PortRanges", Element.Set(PortRangeCollection.Select(
                    a =>
                    a.Item1 == a.Item2
                        ? (Element) Element.IntValue(a.Item1)
                        : (Element) Element.Set(Element.IntValue(a.Item1), Element.IntValue(a.Item2))).ToArray()));
                el.Add(ranges);
            }

            if (SecurePortRangeCollection.Count > 0)
            {
                var sranges = Element.NamedElement("SecurePortRanges", Element.Set(SecurePortRangeCollection.Select(
                    a =>
                    a.Item1 == a.Item2
                        ? (Element) Element.IntValue(a.Item1)
                        : (Element) Element.Set(Element.IntValue(a.Item1), Element.IntValue(a.Item2))).ToArray()));
                el.Add(sranges);
            }
            return el;
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Address}, {PortRanges})";
        }

        internal string SerializeRanges(ICollection<Tuple<int, int>> collection)
        {
            return string.Join(", ", 
                collection.Select(a => a.Item1 == a.Item2
                                 ? a.Item1.ToString()
                                 : $"{a.Item1}-{a.Item2}"));
        }

        internal bool ParsePortRanges(ICollection<Tuple<int, int>> collection, string ports)
        {
            var portranges = ports.Split(',');
            var tempList = new List<Tuple<int, int>>();
            foreach (var t in portranges.Select(p => p.Split('-')))
            {
                if (t.Length == 0)
                    return false;

                int a;

                if (!int.TryParse(t[0], out a))
                    return false;
                
                int b = a;

                if(t.Length > 1 && !int.TryParse(t[1], out b))
                    return false;

                tempList.Add(new Tuple<int, int>(a, b));
            }

            PortRangeCollection.Clear();
            foreach(var e in tempList)
                PortRangeCollection.Add(e);
            return true;
        }
    }
}
