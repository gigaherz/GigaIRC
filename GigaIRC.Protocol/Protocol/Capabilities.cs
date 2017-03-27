using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GigaIRC.Protocol
{
    public class Capabilities
    {
        private readonly Connection parent;

        private readonly List<string> supported = new List<string>();
        private readonly List<string> accepted = new List<string>();

        private string authenticationMechanism = "PLAIN";

        public Capabilities(Connection parent)
        {
            this.parent = parent;
        }

        public void CapabilityResponse(Command cmd)
        {
            if (cmd.Params.Count > 1 && cmd.Params[1] == "LS")
            {
                supported.Clear();
                accepted.Clear();
                supported.AddRange(cmd.Params.Last().ToLowerInvariant().Split(' '));

                var capsRequest = new List<string>();

                if (supported.Contains("multi-prefix"))
                {
                    capsRequest.Add("multi-prefix");
                }

                if (supported.Contains("sasl") && !string.IsNullOrEmpty(parent.NetworkIdentity?.SaslUsername))
                {
                    capsRequest.Add("sasl");
                }

                if (capsRequest.Count > 0)
                {
                    parent.SendLine("CAP REQ :" + string.Join(" ", capsRequest));
                }
                else
                {
                    parent.SendLine("CAP END");
                }
            }
            else if (cmd.Params.Count > 1 && cmd.Params[1] == "ACK")
            {
                accepted.Clear();
                accepted.AddRange(cmd.Params.Last().ToLowerInvariant().Split(' '));

                if (accepted.Contains("sasl") && !string.IsNullOrEmpty(parent.NetworkIdentity?.SaslUsername))
                {
                    parent.SendLine("AUTHENTICATE " + authenticationMechanism);
                }
                else
                {
                    parent.SendLine("CAP END");
                }
            }
            else if (cmd.Params.Count > 1 && cmd.Params[1] == "NAK")
            {
                accepted.RemoveAll(cmd.Params.Last().ToLowerInvariant().Split(' ').ToList().Contains);
            }
        }

        public void AuthenticationRequest(Command cmd)
        {
            if (cmd.Params.Count > 1 && cmd.Params[1] == "+")
            {
                parent.SendLine("AUTHENTICATE " + EncodeSaslPlain(parent.NetworkIdentity.SaslUsername, parent.NetworkIdentity.SaslPassword));
            }
        }

        private string EncodeSaslPlain(string username, string password)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(username + '\x01' + password));
        }
    }
}
