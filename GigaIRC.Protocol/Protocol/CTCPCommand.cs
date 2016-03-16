using System;

namespace GigaIRC.Protocol
{
    public class CTCPCommand
    {
        public readonly string Command;
        public readonly string Text;
        public readonly string Target;

        public CTCPCommand(Command cmd)
        {
            Command = cmd.Params[1].Substring(1, cmd.Params[1].Length - 2);

            Target = cmd.Params[0];

            int pos = Command.IndexOf(' ');
            if (pos >= 0)
            {
                Text = Command.Substring(pos + 1);
                Command = Command.Substring(0, pos);
            }
            else Text = "";
        }

        public bool Is(string cmd)
        {
            return string.Compare(Command, cmd, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
