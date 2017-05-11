using System;
using System.Collections.Generic;
using System.Text;

namespace GigaIRC.Protocol
{
    public class Command
    {
        public UserInfo From { get; set; }
        public string CmdText { get; set; }
        public List<string> Params { get; set; }

        public Command(string cmd, params string[] prms)
        {
            From = new UserInfo();
            CmdText = cmd;
            Params = new List<string>();
            foreach (var t in prms)
            {
                Params.Add(t);
            }
        }

        //Syntax:
        // [:FROM ]COMMAND[ PARAM[ ...]][ :TEXT]
        //
        // If available, text will be added to the params list, and will be the last param
        public Command(string line)
        {
            int p;

            Params = new List<string>();

            var text = "";
            var hasText = false;

            if (line[0] == ':')
            {
                p = line.IndexOf(' ');
                From = new UserInfo(line.Substring(1, p));
                line = line.Substring(p + 1);
            }
            else
                From = new UserInfo();

            p = line.IndexOf(" :", StringComparison.Ordinal);
            if (p > 0)
            {
                text = line.Substring(p + 2);
                line = line.Substring(0, p);
                hasText = true;
            }

            p = line.IndexOf(' ');
            if (p < 0)
            {
                CmdText = line;
                line = "";
            }
            else
            {
                CmdText = line.Substring(0, p);
                line = line.Substring(p + 1);
            }

            while (line.Length>0)
            {
                string cParam;
                p = line.IndexOf(' ');
                if (p < 0)
                {
                    cParam = line;
                    line = "";
                }
                else
                {
                    cParam = line.Substring(0, p);
                    line = line.Substring(p + 1);
                }
                if (cParam.Length > 0)
                    Params.Add(cParam);
            }

            if (hasText)
                Params.Add(text);
        }

        public override string ToString()
        {
            var uinf = From.ToString();
            var cmd = new StringBuilder();

            if (uinf.Length > 0)
            {
                cmd.AppendFormat(":{0} ", uinf);
            }

            cmd.Append(CmdText);

            for (var i = 0; i < Params.Count - 1; i++)
            {
                cmd.Append(" ");
                cmd.Append(Params[i].Replace(' ', '_'));
            }

            var text = Params[Params.Count - 1];
            if (text.IndexOf(' ') >= 0)
            {
                cmd.Append(" :");
                cmd.Append(text);
            }
            else
            {
                cmd.Append(" ");
                cmd.Append(text);
            }

            return cmd.ToString();
        }

        public bool Is(string p)
        {
            return string.Compare(CmdText, p, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
