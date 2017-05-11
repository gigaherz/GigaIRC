using System;
using System.Collections.Generic;
using GigaIRC.Protocol;

namespace GigaIRC.Events
{
    public class LogEventArgs : EventArgs
    {
        public string Text { get; }

        public LogEventArgs(string text)
        {
            Text = text;
        }
    }


    public class FromEventArgs : EventArgs
    {
        public UserInfo From { get; }

        public FromEventArgs(UserInfo from)
        {
            From = from;
        }
    }

    public class TargetEventArgs : FromEventArgs
    {
        public string Target { get; }

        public TargetEventArgs(UserInfo from, string target)
            : base(from)
        {
            Target = target;
        }
    }

    public class TextEventArgs : EventArgs
    {
        public string Text { get; }

        public TextEventArgs(string text)
        {
            Text = text;
        }
    }

    public class MessageEventArgs : TargetEventArgs
    {
        public string Text { get; }

        public MessageEventArgs(UserInfo from, string target, string text)
            : base(from, target)
        {
            Text = text;
        }
    }

    public class KickEventArgs : MessageEventArgs
    {
        public UserInfo Who { get; }

        public KickEventArgs(UserInfo from, string target, UserInfo who, string text)
            : base(from, target, text)
        {
            Who = who;
        }
    }

    public class ModeChangeEventArgs : FromEventArgs
    {
        public string Target { get; }
        public bool Added { get; }
        public string Flag { get; }
        public string Param { get; }

        public ModeChangeEventArgs(UserInfo from, string target, bool added, string flag, string param)
            : base(from)
        {
            Target = target;
            Added = added;
            Flag = flag;
            Param = param;
        }
    }

    public class NamedEventArgs : FromEventArgs
    {
        public string Name { get; }

        public NamedEventArgs(UserInfo from, string name)
            : base(from)
        {
            Name = name;
        }

        public bool Is(string name)
        {
            return string.Compare(Name, name, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    public class CommandEventArgs : NamedEventArgs
    {
        public IList<string> Parameters { get; }

        public CommandEventArgs(Command c)
            : base(c.From, c.CmdText)
        {
            Parameters = c.Params;
        }

    }

    public class CTCPEventArgs : MessageEventArgs
    {
        public string Command { get; }

        public CTCPEventArgs(UserInfo from, CTCPCommand c)
            : base(from, c.Target, c.Text)
        {
            Command = c.Command;
        }

    }

    public class ConnectionStateEventArgs : EventArgs
    {
        public ConnectionState OldState { get; }
        public ConnectionState NewState { get; }

        public ConnectionStateEventArgs(ConnectionState oldState, ConnectionState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }

}
