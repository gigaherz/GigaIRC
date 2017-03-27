using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Events;
using GigaIRC.Protocol;

namespace GigaIRC.Client.WPF.Completion
{
    public class CommandHandler
    {
        public readonly Dictionary<string, InputCommand> CommandRegistry = new Dictionary<string, InputCommand>();
        public InputCommand DefaultCommand { get; set; }
        public InputCommand TextCommand { get; set; }

        public CommandHandler()
        {
            RegisterCommand(new ActionCommand(), "me", "describe");
            RegisterCommand(new MsgCommand(), "msg", "query", "w");
            DefaultCommand = new RawCommand();
            TextCommand = new TextCommand();
        }

        private void RegisterCommand(InputCommand command, params string[] names)
        {
            foreach(var name in names)
                CommandRegistry.Add(name, command);
        }

        public void ProcessCommand(FlexList window, string text)
        {
            if (text.StartsWith("/"))
            {
                var t = text.Substring(1);
                var commandArgs = t.Split(' ');
                var commandName = commandArgs[0].ToLowerInvariant();

                text = t.Substring(commandArgs[0].Length + 1);
                commandArgs = commandArgs.Skip(1).ToArray();

                InputCommand cmd;
                if (!CommandRegistry.TryGetValue(commandName, out cmd))
                {
                    cmd = DefaultCommand;
                }

                cmd?.Handle(commandName, window, text, commandArgs);
            }
            else
            {
                TextCommand?.Handle(null, window, text, null);
            }
        }
    }

    internal class MsgCommand : InputCommand
    {
        public override void Handle(string commandName, FlexList window, string text, string[] args)
        {
            var connection = window.Connection;

            if (args.Length <= 1)
            {
                var status = MainWindow.Instance.WindowManager.Connections[connection].Data.StatusWindow;
                status.AddLine(0, " * ERROR: Nothing to send.");
                window.AddLine(0, " * ERROR: Nothing to send.");
            }

            text = text.Substring(args[0].Length).TrimStart();

            if (MainWindow.Instance.WindowManager.TryGetWindow(connection, args[1], out window))
            {
                var windowId = window.WindowId;
                switch (window.PanelType)
                {
                    case PanelType.Channel:
                        MainWindow.Instance.Connection_OnChannelMessage(connection, new MessageEventArgs(connection.Me, windowId, text));
                        connection.SendMessage(windowId, text);
                        break;
                    case PanelType.Query:
                        MainWindow.Instance.Connection_OnPrivateMessage(connection, new MessageEventArgs(connection.Me, windowId, text));
                        connection.SendMessage(windowId, text);
                        break;
                    default:
                        window.AddLine(0, " * ERROR: Cannot send to window. Use /command to send a command to the server.");
                        break;
                }
            }
            else
            {
                connection.SendMessage(args[0], text);

                var status = MainWindow.Instance.WindowManager.Connections[connection].Data.StatusWindow;
                status.AddLine(0, $"{args[0]} >> {text}");
            }
        }
    }

    internal class TextCommand : InputCommand
    {
        public override void Handle(string commandName, FlexList window, string text, string[] args)
        {
            var connection = window.Connection;
            var windowId = window.WindowId;

            switch (window.PanelType)
            {
                case PanelType.Channel:
                    MainWindow.Instance.Connection_OnChannelMessage(connection, new MessageEventArgs(connection.Me, windowId, text));
                    connection.SendMessage(windowId, text);
                    break;
                case PanelType.Query:
                    MainWindow.Instance.Connection_OnPrivateMessage(connection, new MessageEventArgs(connection.Me, windowId, text));
                    connection.SendMessage(windowId, text);
                    break;
                default:
                    window.AddLine(0, " * ERROR: Cannot send to window. Use /command to send a command to the server.");
                    break;
            }
        }
    }

    internal class RawCommand : InputCommand
    {
        public override void Handle(string commandName, FlexList window, string text, string[] args)
        {
            if (args.Length > 0)
                window.Connection?.SendLine(commandName + " :" + text);
            else
                window.Connection?.SendLine(commandName);
        }
    }

    internal class ActionCommand : InputCommand
    {
        public override void Handle(string commandName, FlexList window, string text, string[] args)
        {
            var connection = window.Connection;
            var windowId = window.WindowId;

            switch (window.PanelType)
            {
                case PanelType.Channel:
                    MainWindow.Instance.Connection_OnChannelAction(connection, new MessageEventArgs(connection.Me, windowId, text));
                    connection.SendCTCP(windowId, "ACTION " + text);
                    break;
                case PanelType.Query:
                    MainWindow.Instance.Connection_OnPrivateAction(connection, new MessageEventArgs(connection.Me, windowId, text));
                    connection.SendCTCP(windowId, "ACTION " + text);
                    break;
                default:
                    window.AddLine(0, " * ERROR: Cannot send to window. Use /command to send a command to the server.");
                    break;
            }
        }
    }

    public abstract class InputCommand
    {
        public abstract void Handle(string commandName, FlexList window, string text, string[] args);
    }
}
