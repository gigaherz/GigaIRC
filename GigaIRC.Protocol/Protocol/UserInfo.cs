namespace GigaIRC.Protocol
{
    public class UserInfo
    {
        public string Nickname { get; internal set; }
        public string Ident { get; internal set; }
        public string Host { get; internal set; }
        public string Address { get; internal set; }
        public ChannelCollection SeenIn { get; private set; } = new ChannelCollection();

        internal UserInfo()
        {
            SeenIn = new ChannelCollection();
            Nickname = "";
            Ident = "";
            Host = "";
            Address = "";
        }

        internal UserInfo(string line)
        {
            int pos = line.IndexOf('@');
            if (pos >= 0)
            {
                Host = line.Substring(pos + 1);
                line = line.Substring(0, pos);
            }
            else Host = "";

            pos = line.IndexOf('!');
            if (pos >= 0)
            {
                Ident = line.Substring(pos + 1);
                line = line.Substring(0, pos);
            }
            else Ident = "";

            Address = "";

            Nickname = line;
        }

        public override string ToString()
        {
            if (Nickname == "")
                return "";

            var c = Nickname;

            if (Ident != "")
                c += "!" + Ident;

            if (Host != "")
                c += "@" + Host;

            return c;

        }

        public bool Is(string p)
        {
            return string.Compare(p, Nickname, true) == 0;
        }

        public bool Is(UserInfo u)
        {
            return Is(u.Nickname);
        }
    }
}
