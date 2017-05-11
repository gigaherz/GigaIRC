using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GigaIRC.Util;
using System.Text.RegularExpressions;

namespace GigaIRC.Client.WPF.Util
{
    public class LineToParagraphConverter
    {
        public static Paragraph ToParagraph(LineInfo line, ICommand linkClickCommand, Regex nicknamesRegex, Paragraph tb = null)
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

            var words = WordInfo.Split(line.Line, line.Color, nicknamesRegex);
            foreach (var word in words)
            {
                var span = new Run();
                
                if (word.IsBold) span.FontWeight = FontWeights.Bold;
                if (word.IsUnderline) span.TextDecorations = TextDecorations.Underline;
                if (word.IsItalic) span.FontStyle = FontStyles.Italic;

                if (word.IsReverse ||
                    word.ForeColor != line.Color ||
                    word.BackColor != ColorTheme.Background)
                {
                    span.Foreground = ColorTheme.Brush(word.ForeColor);
                    span.Background = ColorTheme.Brush(word.BackColor);
                }

                string text = word.Word;

                span.Text = text;

                if (word.Link != null)
                {
                    var link = new Hyperlink
                    {
                        NavigateUri = word.Link, Command = linkClickCommand, CommandParameter = word.Link
                    };
                    link.Inlines.Add(span);
                    tb.Inlines.Add(link);
                }
                else
                {
                    tb.Inlines.Add(span);
                }
            }

            return tb;
        }
    }
}