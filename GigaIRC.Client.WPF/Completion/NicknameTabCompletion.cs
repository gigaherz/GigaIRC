using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Protocol;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace GigaIRC.Client.WPF.Completion
{
    class NicknameTabCompletion : ITabCompletion
    {
        private static readonly char[] Whitespace = { ' ', '\t' };

        private readonly FlexList target;

        private string prefix;
        private List<string> rotation;

        public NicknameTabCompletion(FlexList box)
        {
            target = box;
        }

        public bool TabPressed()
        {
            var text = target.Input.Text.ToLowerInvariant();
            var before = text.Substring(0, target.Input.SelectionStart);

            var wordStart = before.LastIndexOfAny(Whitespace) + 1;

            if (wordStart < 0) wordStart = 0;

            var wordAndTail = text.Substring(wordStart);

            // Don't use as the length, may be -1. Use word.Length instead
            var wordEnd = wordAndTail.IndexOfAny(Whitespace);

            var word = wordEnd < 0 ? wordAndTail : wordAndTail.Substring(0, wordEnd);

            // If no word, we don't want to do anything
            if (word.Length == 0)
                return false;

            // If we don't have an active rotation, we need a new prefix
            if (string.IsNullOrEmpty(prefix) || !word.StartsWith(prefix))
            {
                prefix = word;
                rotation = null;
            }

            // We may have to re-compute the rotation list even if the prefix hasn't changed (quit/part/join/kick/nickchange)
            if (rotation == null)
            {
                rotation = (target.ItemsSource as ChannelUserCollection)
                    .Select(e => e.User.Nickname)
                    .Where(nick => nick.ToLowerInvariant()
                    .StartsWith(prefix))
                    .ToList();
            }

            if (rotation.Count == 0)
                return false;

            var wordNumber = (rotation.FindIndex(s => string.Compare(s, word, StringComparison.OrdinalIgnoreCase) == 0) + 1) % rotation.Count;

            if (wordNumber < 0) wordNumber = 0;

            var newWord = rotation[wordNumber];

            target.Input.Select(wordStart, word.Length);
            target.Input.SelectedText = newWord;
            target.Input.Select(wordStart + newWord.Length, 0);

            return true; // Tab rotation successful, accept and eat the keypress.
        }

        public void TextChanged()
        {
            prefix = null;
        }

        public void ListChanged()
        {
            rotation = null;
        }
    }
}
