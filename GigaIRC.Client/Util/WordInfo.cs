using System.Collections.Generic;

namespace GigaIRC.Util
{
    internal class WordInfo
    {
        public ColorCode ForeColor { get; }
        public ColorCode BackColor { get; }

        public bool IsBold { get; }
        public bool IsUnderline { get; }
        public bool IsReverse { get; }

        public string Word { get; }

        public WordInfo(ColorCode back, ColorCode fore, bool bold, bool under, bool rev, string wrd)
        {
            ForeColor = fore;
            BackColor = back;
            IsBold = bold;
            IsUnderline = under;
            IsReverse = rev;
            Word = wrd;
        }

        public static IEnumerable<WordInfo> Split(string line, ColorCode defColor)
        {
            char[] chars = (line + ((char)0) + ((char)0)).ToCharArray();

            bool isBold = false;
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

                    switch ((int)chars[i])
                    {
                        case 2:
                            newWord = true;
                            break;
                        case 3:
                            newWord = true;
                            break;
                        case 15: // clear codes
                            newWord = true;
                            break;
                        case 22: // reverse
                            newWord = true;
                            break;
                        case 31:
                            newWord = true;
                            break;
                    }

                    if (newWord)
                    {
                        yield return new WordInfo(cback, cfore, isBold, isUnderline, isReverse, word);
                        word = "";
                    }

                    switch ((int)chars[i])
                    {
                        case 0:
                            break; // ignore null chars
                        case 2:
                            isBold = !isBold;
                            break;
                        case 3:
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
                        case 15: // clear codes
                            cback = ColorTheme.Background;
                            cfore = defColor;
                            isBold = false;
                            isUnderline = false;
                            isReverse = false;
                            break;
                        case 22: // reverse
                            isReverse = !isReverse;
                            {
                                var t = cfore;
                                cfore = cback;
                                cback = t;
                            }
                            break;
                        case 31:
                            //?
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
                yield return new WordInfo(cback, cfore, isBold, isUnderline, isReverse, word);
            }
        }
    }
}