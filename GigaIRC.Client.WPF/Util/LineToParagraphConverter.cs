using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace GigaIRC.Util
{
    public class LineToParagraphConverter : IValueConverter
    {
        public static LineToParagraphConverter Instance { get; } = new LineToParagraphConverter();

        private LineToParagraphConverter() { }

        public static Paragraph ToParagraph(LineInfo line, Paragraph tb = null)
        {
            if (tb == null) tb = new Paragraph
            {
                Foreground = ColorTheme.Brush(line.Color),
                Padding = new Thickness(),
                Margin = new Thickness()
            };
            else
            {
                tb.Foreground = ColorTheme.Brush(line.Color);
                tb.Inlines.Clear();
            }

            var words = WordInfo.Split(line.Line, line.Color);
            foreach (var word in words)
            {
                var span = new Run();

                if (word.IsBold) span.FontWeight = FontWeights.Bold;
                if (word.IsUnderline) span.TextDecorations = TextDecorations.Underline;

                if (word.IsReverse ||
                    word.ForeColor != line.Color ||
                    word.BackColor != ColorTheme.Background)
                {
                    span.Foreground = ColorTheme.Brush(word.ForeColor);
                    span.Background = ColorTheme.Brush(word.BackColor);
                }

                string t = word.Word;

                span.Text = t;

                tb.Inlines.Add(span);
            }

            return tb;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var line = value as LineInfo;
            if (line == null)
                return value.ToString();
            return ToParagraph(line);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}