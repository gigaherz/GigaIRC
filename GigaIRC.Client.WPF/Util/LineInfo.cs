
namespace GigaIRC.Util
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