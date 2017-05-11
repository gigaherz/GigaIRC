using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GigaIRC.Protocol
{
    public class Capabilities
    {
        private readonly Connection _parent;

        private readonly List<string> _supported = new List<string>();
        private readonly List<string> _accepted = new List<string>();

        private readonly string _authenticationMechanism = "PLAIN";

        public Capabilities(Connection parent)
        {
            this._parent = parent;
        }

        public void CapabilityResponse(Command cmd)
        {
            if (cmd.Params.Count > 1 && cmd.Params[1] == "LS")
            {
                _supported.Clear();
                _accepted.Clear();
                _supported.AddRange(cmd.Params.Last().ToLowerInvariant().Split(' '));

                var capsRequest = new List<string>();

                if (_supported.Contains("multi-prefix"))
                {
                    capsRequest.Add("multi-prefix");
                }

                if (_supported.Contains("sasl") && !string.IsNullOrEmpty(_parent.NetworkIdentity?.SaslUsername))
                {
                    capsRequest.Add("sasl");
                }

                if (capsRequest.Count > 0)
                {
                    _parent.SendLine("CAP REQ :" + string.Join(" ", capsRequest));
                }
                else
                {
                    _parent.SendLine("CAP END");
                }
            }
            else if (cmd.Params.Count > 1 && cmd.Params[1] == "ACK")
            {
                _accepted.Clear();
                _accepted.AddRange(cmd.Params.Last().ToLowerInvariant().Split(' '));

                if (_accepted.Contains("sasl") && !string.IsNullOrEmpty(_parent.NetworkIdentity?.SaslUsername))
                {
                    _parent.SendLine("AUTHENTICATE " + _authenticationMechanism);
                }
                else
                {
                    _parent.SendLine("CAP END");
                }
            }
            else if (cmd.Params.Count > 1 && cmd.Params[1] == "NAK")
            {
                _accepted.RemoveAll(cmd.Params.Last().ToLowerInvariant().Split(' ').ToList().Contains);
            }
        }

        public void AuthenticationRequest(Command cmd)
        {
            if (cmd.Params.Count > 1 && cmd.Params[1] == "+")
            {
                _parent.SendLine("AUTHENTICATE " + EncodeSaslPlain(_parent.NetworkIdentity.SaslUsername, _parent.NetworkIdentity.SaslPassword));
            }
        }

        private string EncodeSaslPlain(string username, string password)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(username + '\x01' + password));
        }
    }
}
