using System.Windows.Media;
using GigaIRC.Util;

namespace GigaIRC.Client.WPF.Util
{
    public static class ColorTheme
    {
        public static readonly Color[] Palette = {
            Colors.White,        Colors.Black,        Colors.DarkBlue,     Colors.DarkGreen,
            Colors.Red,          Colors.DarkRed,      Colors.DarkMagenta,  Colors.Orange,
            Colors.Yellow,       Colors.Green,        Colors.Cyan,         Colors.SteelBlue,
            Colors.DodgerBlue,   Colors.Tomato,       Colors.Gray,         Colors.LightGray
        };

        public static Brush Brush(ColorCode idx)
        {
            var color = Palette[(int)idx];

            return new SolidColorBrush(color);
        }

        public static ColorCode Background { get; set; } = ColorCode.Black;
        public static ColorCode Join { get; set; } = ColorCode.Green;
        public static ColorCode Part { get; set; } = ColorCode.Blue;
        public static ColorCode Quit { get; set; } = ColorCode.Red;
        public static ColorCode Kick { get; set; } = ColorCode.Red;
        public static ColorCode Nick { get; set; } = ColorCode.Blue;
        public static ColorCode Mode { get; set; } = ColorCode.Orange;
        public static ColorCode Topic { get; set; } = ColorCode.LightGray;
        public static ColorCode TopicInfo { get; set; } = ColorCode.LightGray;
        public static ColorCode TextOthers { get; set; } = ColorCode.White;
        public static ColorCode TextOwn { get; set; } = ColorCode.LightGray;
        public static ColorCode NoticeOthers { get; set; } = ColorCode.Orange;
        public static ColorCode NoticeOwn { get; set; } = ColorCode.Orange;
        public static ColorCode ActionOthers { get; set; } = ColorCode.Orange;
        public static ColorCode ActionOwn { get; set; } = ColorCode.Orange;
        public static ColorCode Default { get; set; } = ColorCode.LightGray;
        public static ColorCode Ctcp { get; set; } = ColorCode.Blue;
        public static ColorCode CtcpReply { get; set; } = ColorCode.Blue;

        public static Brush BackgroundBrush => Brush(Background);
        public static Brush JoinBrush => Brush(Join);
        public static Brush PartBrush => Brush(Part);
        public static Brush QuitBrush => Brush(Quit);
        public static Brush KickBrush => Brush(Kick);
        public static Brush NickBrush => Brush(Nick);
        public static Brush ModeBrush => Brush(Mode);
        public static Brush TopicBrush => Brush(Topic);
        public static Brush TopicInfoBrush => Brush(TopicInfo);
        public static Brush TextOthersBrush => Brush(TextOthers);
        public static Brush TextOwnBrush => Brush(TextOwn);
        public static Brush NoticeOthersBrush => Brush(NoticeOthers);
        public static Brush NoticeOwnBrush => Brush(NoticeOwn);
        public static Brush ActionOthersBrush => Brush(ActionOthers);
        public static Brush ActionOwnBrush => Brush(ActionOwn);
        public static Brush DefaultBrush => Brush(Default);

    }
}
