using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDDL.Structure;
using GigaIRC.Protocol;

namespace GigaIRC.Config
{
    public class Network
    {
        public string Name { get; set; }

        public readonly List<Server> Servers = new List<Server>();

        public string Password { get; set; }

        public Identity DefaultIdentity { get; set; }

        public Network()
        {
        }

        public Network(Set net)
        {
            Element named;

            if (net.TryGetValue("Name", out named))
            {
                var name = named as Value;
                if (name == null)
                    throw new InvalidDataException();

                Name = name.String;
            }

            if (net.TryGetValue("Password", out named))
            {
                var pass = named as Value;
                if (pass == null)
                    throw new InvalidDataException();

                Password = pass.String;
            }

            if (net.TryGetValue("DefaultIdentity", out named))
            {
                //var name = named as ValueElement;
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

            var name = Element.NamedElement("Name", Element.StringValue(Name));
            el.Add(name);

            if (Password != null)
            {
                var password = Element.NamedElement("Password", Element.StringValue(Password));
                el.Add(password);
            }

            if (DefaultIdentity != null)
            {
                var defIdentity = Element.NamedElement("DefaultIdentity",
                                                       Element.StringValue(DefaultIdentity.DescriptiveName));
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
