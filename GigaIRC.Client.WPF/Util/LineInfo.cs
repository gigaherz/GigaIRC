
using GigaIRC.Util;

namespace GigaIRC.Client.WPF.Util
{
    public class LineInfo
    {
        public ColorCode Color { get; }
        public string Line { get; }

        public LineInfo(ColorCode c, string l)
        {
            Color = c;
            Line = l;
        }
    }
}