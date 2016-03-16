namespace GigaIRC.Protocol
{
    public class ChannelMode
    {
        public readonly char Key;
        public readonly string Value;

        public ChannelMode(char key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}