using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using GigaIRC.Util;
using UserControl = System.Windows.Forms.UserControl;

namespace GigaIRC.Client.RichList
{
    public partial class WpfRichList : UserControl
    {
        public readonly ObservableCollection<LineInfo> Lines = new ObservableCollection<LineInfo>();

        FlowDocument content;
        System.Windows.Controls.RichTextBox richText;

        public WpfRichList()
        {
            InitializeComponent();
            
            var bg = ColorTheme.Brush(ColorTheme.Background);
            richText = new System.Windows.Controls.RichTextBox
            {
                Background = bg,
                IsReadOnly = true,
                IsReadOnlyCaretVisible = false,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12.5
            };

            content = richText.Document;
            content.Blocks.Clear();

            elementHost1.Child = richText;
        }

        // IRCList interface implementation
        public void Clear()
        {
            Lines.Clear();

            content?.Blocks.Clear();
        }

        public void AddLine(int color, string text)
        {
            if (IsDisposed)
                return;

            if (content == null)
                return;

            var line = new LineInfo((ColorCode) color, text);
            Lines.Add(line);

            BeginInvoke(new Action(() =>
            {
                content.Blocks.Add(LineToParagraphConverter.ToParagraph(line));

                //if(shouldScroll)
                richText.ScrollToEnd();
            }));
        }

        public void AddLine(int before, int color, string text)
        {
            if (IsDisposed)
                return;

            if (content == null)
                return;

            var line = new LineInfo((ColorCode)color, text);
            Lines.Insert(before, line);

            BeginInvoke(new Action(() =>
            {
                if (before == content.Blocks.Count)
                {
                    content.Blocks.Add(LineToParagraphConverter.ToParagraph(line));
                }
                else
                {
                    var after = content.Blocks.ElementAt(before);
                    content.Blocks.InsertBefore(after, LineToParagraphConverter.ToParagraph(line));
                }
            }));
        }

        public void SetLine(int number, int color, string text)
        {
            if (IsDisposed)
                return;

            if (content == null)
                return;

            var line = new LineInfo((ColorCode)color, text);
            Lines[number] = line;

            BeginInvoke(new Action(() =>
            {
                var at = content.Blocks.ElementAt(number);
                LineToParagraphConverter.ToParagraph(line, (Paragraph)at);
            }));
        }

        public void RemoveLine(int number)
        {
            Lines.RemoveAt(number);

            BeginInvoke(new Action(() =>
            {
                content.Blocks.Remove(content.Blocks.ElementAt(number));
            }));
        }

        public string GetLine(int number)
        {
            return Lines[number].Line;
        }

        public int GetLineColor(int number)
        {
            return (int)Lines[number].Color;
        }

        public int LineCount()
        {
            return Lines.Count;
        }
    }
}
