﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDDL.Config;
using GDDL.Structure;
using GigaIRC.Util;

namespace GigaIRC.Config
{
    public class Settings
    {
        private readonly List<Network> networks = new List<Network>();
        private readonly List<Identity> identities = new List<Identity>();

        public Identity DefaultIdentity { get; set; }

        public ICollection<Network> Networks => networks;

        public ICollection<Identity> Identities => identities;

        public void LoadFromFile(string fileName)
        {
            var parser = GDDL.Parser.FromFile(fileName);
            var data = parser.Parse() as Set;
            if (data == null)
                return;

            Element netsElement;
            if(data.TryGetValue("Networks", out netsElement))
            {
                var nets = netsElement as Set;
                if (nets == null)
                    throw new InvalidDataException();

                foreach (var net in nets.ByType("network"))
                {
                    networks.Add(new Network(net));
                }
            }

            Element idsElement;
            if (data.TryGetValue("Identities", out idsElement))
            {
                var ids = idsElement as Set;
                if (ids == null)
                    throw new InvalidDataException();

                foreach (var ident in ids
                    .Select<Element, Set>(a => a as Set)
                    .Where(a => a != null)
                    .Where(a => string.Compare(a.TypeName, "identity", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    identities.Add(new Identity(ident));
                }
            }
        }

        public void SaveToFile(string fileName)
        {
            var data = new Set();

            var nets = Element.Set(networks.Select(network => network.ToConfigString()));
            data.Add(Element.NamedElement("Networks", nets));

            var ids = Element.Set(identities.Select(identity => identity.ToConfigString()));
            data.Add(Element.NamedElement("Identities", ids));

            if (DefaultIdentity != null)
            {
                var def = Element.Backreference(true, $"identities:{DefaultIdentity.Nickname}");
                data.Add(Element.NamedElement("DefaultIdentity", def));
            }

            File.WriteAllText(fileName, data.ToString(new StringGenerationContext(StringGenerationOptions.Nice)));
        }

        public void ImportMircServersIni(string path)
        {
            //H:\mIRC\servers.ini

            networks.Clear();
            identities.Clear();
            DefaultIdentity = null;

            var ini = new IniFile(path);

            for(int i = 0;;i++)
            {
                const string section = "networks";
                string key = $"n{i}";

                var network = ini.Read(section, key, null);
                if(network == null)
                    break;

                var net = new Network {Name = network};

                networks.Add(net);
            }

            for (int i = 0; ; i++)
            {
                const string section = "servers";
                string key = $"n{i}";

                var info = ini.Read(section, key, null);
                if (info == null)
                    break;

                //[EU, NL, Amsterdam]SERVER:[efnet.xs4all.nl]:[6661-6669]GROUP:[EFnet]

                var data = info.Split(new [] {"SERVER:", "GROUP:", ":"}, StringSplitOptions.RemoveEmptyEntries);

                var name = data[0];
                var addr = data[1];
                var ports = data[2];
                var network = data[3];


                var net = networks.Find(n => string.Compare(n.Name, network, true) == 0);
                if(net == null)
                {
                    net = new Network {Name = network};
                    networks.Add(net);
                }

                var svr = new Server(net) {DisplayName = name, Address = addr};

                var portranges = ports.Split(',');
                foreach(var p in portranges)
                {
                    var t = p.Split('-');
                    var a = int.Parse(t[0]);

                    if(t.Length==1)
                    {
                        svr.PortRanges.Add(new Tuple<int, int>(a, a));
                    }
                    else
                    {
                        var b = int.Parse(t[1]);
                        svr.PortRanges.Add(new Tuple<int, int>(a, b));
                    }
                }

                net.Servers.Add(svr);
            }
        }
    }
}