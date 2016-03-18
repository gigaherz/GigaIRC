using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using GigaIRC.Client.WPF;

namespace GigaIRC.Util
{
    public class LineToParagraphConverter
    {
        public static Paragraph ToParagraph(LineInfo line, ICommand linkClickCommand, Paragraph tb = null)
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
                if (word.IsItalic) span.FontStyle = FontStyles.Italic;

                if (word.IsReverse ||
                    word.ForeColor != line.Color ||
                    word.BackColor != ColorTheme.Background)
                {
                    span.Foreground = ColorTheme.Brush(word.ForeColor);
                    span.Background = ColorTheme.Brush(word.BackColor);
                }

                string t = word.Word;

                span.Text = t;

                if (word.IsLink)
                {
                    Uri uri;

                    if (word.Word.Contains(":/"))
                        uri = new Uri(word.Word);
                    else if(word.Word.StartsWith("ftp."))
                        uri = new Uri("ftp://" + word.Word);
                    else
                        uri = new Uri("http://" + word.Word);

                    var link = new Hyperlink
                    {
                        NavigateUri = uri
                    };
                    link.RequestNavigate += (sender, args) =>
                    {
                        if (linkClickCommand != null && linkClickCommand.CanExecute(args.Uri))
                        {
                            linkClickCommand.Execute(args.Uri);
                            args.Handled = true;
                        }
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