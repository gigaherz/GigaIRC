using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GigaIRC.Client.WPF.Util;

namespace GigaIRC.Util
{
    internal class WordInfo
    {
        private static readonly Regex FindUrl = new Regex(@"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ColorCode ForeColor { get; }
        public ColorCode BackColor { get; }

        public bool IsBold { get; }
        public bool IsItalic { get; }
        public bool IsUnderline { get; }
        public bool IsReverse { get; }

        public Uri Link { get; }

        public string Word { get; }

        public WordInfo(ColorCode back, ColorCode fore, bool bold, bool italic, bool under, bool rev, Uri link, string wrd)
        {
            ForeColor = fore;
            BackColor = back;
            IsBold = bold;
            IsItalic = italic;
            IsUnderline = under;
            IsReverse = rev;
            Link = link;
            Word = wrd;
        }

        public static IEnumerable<WordInfo> Split(string line, ColorCode defColor)
        {
            char[] chars = (line + (char)0 + (char)0).ToCharArray();

            bool isBold = false;
            bool isItalic = false;
            bool isUnderline = false;
            bool isReverse = false;
            ColorCode cfore = defColor;
            ColorCode cback = ColorTheme.Background;

            string word = "";

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] < 32)
                {
                    bool newWord = false;

                    switch (chars[i])
                    {
                        case EscapeCodes.Bold:
                        case EscapeCodes.Color:
                        case EscapeCodes.Reset:
                        case EscapeCodes.Reverse:
                        case EscapeCodes.Italic:
                        case EscapeCodes.Underline:
                            newWord = true;
                            break;
                    }

                    if (newWord)
                    {
                        foreach (var e in FindLinks(cback, cfore, isBold, isItalic, isUnderline, isReverse, word))
                            yield return e;
                        word = "";
                    }

                    switch ((int)chars[i])
                    {
                        case 0:
                            break; // ignore null chars
                        case EscapeCodes.Bold:
                            isBold = !isBold;
                            break;
                        case EscapeCodes.Color:
                            var foreColor = -1;
                            var backColor = (int)ColorTheme.Background;

                            do
                            {
                                char cNext = chars[i + 1];
                                if (char.IsDigit(cNext))
                                {
                                    foreColor = cNext - '0';
                                    i++;

                                    cNext = chars[i + 1];
                                    if (char.IsDigit(cNext))
                                    {
                                        foreColor = foreColor * 10 + cNext - '0';
                                        i++;
                                    }

                                    if (cNext != ',')
                                        continue;

                                    char cNext2 = chars[i + 2];
                                    if (!char.IsDigit(cNext2))
                                        continue;

                                    backColor = cNext2 - '0';
                                    i += 2;

                                    cNext = chars[i + 1];
                                    if (!char.IsDigit(cNext))
                                        continue;

                                    backColor = backColor * 10 + cNext - '0';

                                    i++;
                                }
                            } while (false);

                            if (foreColor == -1)
                            {
                                cback = ColorTheme.Background;
                                cfore = defColor;
                            }
                            else
                            {
                                if (isReverse)
                                {
                                    cback = (ColorCode)foreColor;
                                    cfore = (ColorCode)backColor;
                                }
                                else
                                {
                                    cfore = (ColorCode)foreColor;
                                    cback = (ColorCode)backColor;
                                }
                            }
                            break;
                        case EscapeCodes.Reset:
                            cback = ColorTheme.Background;
                            cfore = defColor;
                            isBold = false;
                            isItalic = false;
                            isUnderline = false;
                            isReverse = false;
                            break;
                        case EscapeCodes.Reverse:
                            isReverse = !isReverse;
                            {
                                var t = cfore;
                                cfore = cback;
                                cback = t;
                            }
                            break;
                        case EscapeCodes.Italic:
                            isItalic = !isItalic;
                            break;
                        case EscapeCodes.Underline:
                            isUnderline = !isUnderline;
                            break;
                    }
                }
                else
                {
                    word += chars[i];
                }

            }

            if (word != "")
            {
                foreach(var e in FindLinks(cback, cfore, isBold, isItalic, isUnderline, isReverse, word))
                    yield return e;
            }
        }

        private static IEnumerable<WordInfo> FindLinks(ColorCode back, ColorCode fore, bool bold, bool italic, bool under, bool rev, string wrd)
        {
            var match = FindUrl.Match(wrd);
            while (match.Success)
            {
                var g = match.Groups[0];

                Uri uri = null;
                try
                {
                    var text = g.Value;

                    if (text.Contains(":/"))
                        uri = new Uri(text);
                    else if (text.StartsWith("ftp."))
                        uri = new Uri("ftp://" + text);
                    else
                        uri = new Uri("http://" + text);
                }
                catch (Exception)
                {
                    // ignore
                }

                var idx = g.Index;
                var len = g.Length;
                if (uri != null)
                {
                    if (idx > 0)
                    {
                        yield return new WordInfo(back, fore, bold, italic, under, rev, null, wrd.Substring(0, idx));
                    }
                    yield return new WordInfo(back, fore, bold, italic, under, rev, uri, g.Value);
                }
                else
                {
                    yield return new WordInfo(back, fore, bold, italic, under, rev, null, wrd.Substring(0, idx+len));
                }

                wrd = wrd.Substring(idx + len);
                match = FindUrl.Match(wrd);
            }

            if(wrd.Length > 0)
                yield return new WordInfo(back, fore, bold, italic, under, rev, null, wrd);
        }
    }
}