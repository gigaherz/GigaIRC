using System.Linq;

namespace GigaIRC.Protocol
{
    public class Capabilities
    {
        readonly Connection parent;

        public Capabilities(Connection parent)
        {
            this.parent = parent;
        }

        public void Received(Command cmd)
        {
            if (cmd.Params.Count > 1 && cmd.Params[1] == "LS")
            {
                //"CAP REQ :multi-prefix sasl"

                int caps = 0;
                string capsRequest = "";

                if (cmd.Params.Last().Contains("multi-prefix"))
                {
                    caps = 1;
                    capsRequest = "multi-prefix";
                }

                if (caps > 0)
                {
                    parent.SendLine("CAP REQ :" + capsRequest);
                }
                else
                {
                    parent.SendLine("CAP END");
                }
            }
            else if (cmd.Params.Count > 1 && cmd.Params[1] == "ACK")
            {
                parent.SendLine("CAP END");
            }
        }
    }
}
