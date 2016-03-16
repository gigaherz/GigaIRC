using System;
using System.Collections.Generic;
using GDDL.Structure;
using GigaIRC.Protocol;

namespace GigaIRC.Config
{
    public class Identity
    {
        public string DescriptiveName { get; set; }

        public string FullName { get; set; }
        public string Username { get; set; }

        public string Nickname { get; set; }
        public string AltNickname { get; set; }

        public readonly HashSet<Network> Networks = new HashSet<Network>();

        public Identity()
        {
        }

        public Identity(Set ident)
        {
            throw new NotImplementedException();
        }

        internal Element ToConfigString()
        {
            throw new NotImplementedException();
        }
    }
}
