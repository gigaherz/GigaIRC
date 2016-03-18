using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using GigaIRC.Client.WPF.Dialogs;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Client.WPF.Util;
using GigaIRC.Config;
using GigaIRC.Events;
using GigaIRC.Protocol;
using GigaIRC.Util;
using Xceed.Wpf.AvalonDock.Layout;
using WebBrowser = GigaIRC.Client.WPF.Dockable.WebBrowser;

namespace GigaIRC.Client.WPF
{
    public partial class MainWindow
    {
        LayoutAnchorable _preferencesPane;
        LayoutAnchorable _debugPane;

        public static MainWindow Instance { get; private set; }

        public Session Session { get; } = new Session();

        private readonly Dictionary<Connection, Dictionary<string, FlexList>> _connectionWindows = new Dictionary<Connection, Dictionary<string, FlexList>>();
        private readonly Dictionary<string, FlexList> _specialWindows = new Dictionary<string, FlexList>();

        public FlexList this[Connection cn, string wndName]
        {
            get
            {
                var dict = cn == null ? _specialWindows : _connectionWindows[cn];
                return dict[wndName];
            }
            private set
            {
                var dict = cn == null ? _specialWindows : _connectionWindows[cn];
                dict[wndName] = value;
            }
        }

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            ShowDebug();

            Session.Settings.LoadFromFile(@"settings.cfg");

            AttachEvents();

            this[null, "@@debug@@"].AddLine(0, "Program Initialization Finished.");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var conn in Session.Connections.ToList())
                conn.Close();
        }

        LayoutAnchorable AttachDockable(DockableBase p)
        {
            var anchorable = new LayoutAnchorable
            {
                Content = p,
                Title = p.Title
            };

            p.AnchorableParent = anchorable;

            p.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Title")
                    p.AnchorableParent.Title = p.Title;
            };

            anchorable.Closed += (sender, args) => p.OnClosed();
            anchorable.Closing += (sender, args) => args.Cancel = p.OnClosing();

            MainContent.Children.Add(anchorable);

            return anchorable;
        }

        public RelayCommand OpenPreferencesCommand => new RelayCommand(_ => OpenPreferences());
        void OpenPreferences()
        {
            if (_preferencesPane == null)
            {
                var preferences = new Dockable.Preferences();
                _preferencesPane = AttachDockable(preferences);

                _preferencesPane.Closed += (sender, args) => _preferencesPane = null;
            }
            DockingManager.ActiveContent = _preferencesPane;
        }

        public RelayCommand ShowDebugCommand => new RelayCommand(_ => ShowDebug());
        void ShowDebug()
        {
            if (_debugPane == null)
            {
                var debugPanel = new FlexList { ShowTopic = false, ShowListbox = false, Title="Debug" };
                _debugPane = AttachDockable(debugPanel);
                debugPanel.LinkClickedCommand = new RelayCommand(LinkClicked);

                debugPanel.OnInput += (sender, args) => debugPanel.AddLine((int)ColorTheme.Default, args.Text);
                _debugPane.Closed += (sender, args) => _debugPane = null;

                _specialWindows["@@debug@@"] = debugPanel;
            }
            DockingManager.ActiveContent = _debugPane;
        }

        public RelayCommand NewBrowserCommand => new RelayCommand(_ => NewBrowser());
        void NewBrowser(Uri url = null)
        {
            var browser = new WebBrowser();
            var pane = AttachDockable(browser);

            if(url != null)
                browser.Browser.Source = url;
            
            DockingManager.ActiveContent = pane;
        }

        public RelayCommand NewConnectionCommand => new RelayCommand(_ => NewConnection());
        void NewConnection()
        {
            var wnd = new QuickConnect {Owner = this};
            if (wnd.ShowDialog() == true)
            {

                var id = new Identity
                {
                    FullName = wnd.FullName,
                    Username = wnd.Username,
                    Nickname = wnd.Nickname,
                    AltNickname = wnd.AltNickname,
                };

                var svr = new Server(new Network { Name = "Test" })
                {
                    Address = wnd.Server
                };

                svr.PortRanges.Add(new Tuple<int, int>(6667, 6667));

                Session.ConnectTo(svr, id);
            }
        }

        #region Window Management
        private FlexList CreateWindow(Connection cn, string id, string title)
        {
            var panel = new FlexList { Connection = cn, WindowId = id };
            panel.OnInput += Window_OnInput;

            panel.LinkClickedCommand = new RelayCommand(LinkClicked);

            AttachDockable(panel);

            this[cn, id] = panel;

            panel.Title = title;
            panel.Closed += (sender, args) => RemoveWindow(panel.Connection, panel.WindowId);

            return panel;
        }

        private void LinkClicked(object obj)
        {
            NewBrowser(obj as Uri);
        }

        private void RemoveWindow(Connection cn, string id)
        {
            var dict = cn == null ? _specialWindows : _connectionWindows[cn];
            dict.Remove(id);
        }

        private bool ContainsWindow(Connection cn, string id)
        {
            var dict = cn == null ? _specialWindows : _connectionWindows[cn];
            return dict.ContainsKey(id);
        }

        private bool TryGetWindow(Connection cn, string id, out FlexList window)
        {
            var dict = cn == null ? _specialWindows : _connectionWindows[cn];
            return dict.TryGetValue(id, out window);
        }
        #endregion


        #region Input Handler

        private void Window_OnInput(object sender, TextInputEventArgs e)
        {
            var d = (FlexList)sender;
            var connection = d.Connection;
            var windowId = d.WindowId;

            if (e.Text.Substring(0, 1) == "/")
            {
                //parse command
                var t = e.Text.Substring(1);
                var cmd = t.Split(' ');

                if (string.Compare(cmd[0], "me", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    e.Text = e.Text.Substring(4);
                    if (windowId == "@@status@@")
                    {
                        d.AddLine(0, " * ERROR: Cannot send to window. Use /command to send a command to the server.");
                    }
                    else
                    {
                        if (windowId[0] == '#')
                        {
                            Connection_OnChannelAction(connection, new MessageEventArgs(connection.Me, windowId, e.Text));
                        }
                        else
                        {
                            Connection_OnPrivateAction(connection, new MessageEventArgs(connection.Me, windowId, e.Text));
                        }
                        connection.SendCTCP(windowId, "ACTION " + e.Text);
                    }
                }
                else
                {
                    connection?.SendLine(t);
                }
            }
            else
            {
                if (windowId.StartsWith("@@"))
                {
                    d.AddLine(0, " * ERROR: Cannot send to window. Use /command to send a command to the server.");
                }
                else
                {
                    if (connection.PreChars.Contains(windowId[0]))
                    {
                        Connection_OnChannelMessage(connection, new MessageEventArgs(connection.Me, windowId, e.Text));
                    }
                    else
                    {
                        Connection_OnPrivateMessage(connection, new MessageEventArgs(connection.Me, windowId, e.Text));
                    }
                    connection.SendMessage(windowId, e.Text);
                }
            }
        }

        #endregion

        public void AttachEvents()
        {
            // NOTE: If you add an event here, remember to detach it below!

            Session.Connections.CollectionChanged += Connections_CollectionChanged;

            Session.OnRawCommand.Add(Connection_OnTopicText, "332");
            Session.OnRawCommand.Add(Connection_OnTopicInfo, "333");

            Session.OnJoin += Connection_OnJoin;
            Session.OnPart += Connection_OnPart;
            Session.OnQuit += Connection_OnQuit;
            Session.OnKick += Connection_OnKick;

            Session.OnNickChange += Connection_OnNickChange;

            Session.OnChannelMessage += Connection_OnChannelMessage;
            Session.OnChannelNotice += Connection_OnChannelNotice;
            Session.OnChannelAction += Connection_OnChannelAction;
            Session.OnChannelTopic += Connection_OnChannelTopic;
            Session.OnChannelCtcp += Connection_OnChannelCtcp;
            Session.OnChannelCtcpReply += Connection_OnChannelCtcpReply;


            Session.OnPrivateMessage += Connection_OnPrivateMessage;
            Session.OnPrivateNotice += Connection_OnPrivateNotice;
            Session.OnPrivateAction += Connection_OnPrivateAction;
            Session.OnPrivateCtcp += Connection_OnPrivateCtcp;
            Session.OnPrivateCtcpReply += Connection_OnPrivateCtcpReply;

            Session.OnChannelModes += Connection_OnChannelModes;

            Session.OnLogLine += Debug_OnLog;

            // TODO
            //Session.OnChannelModeChange += Connection_OnChannelModeChange;
            //Session.OnUserModeChange += Connection_OnUserModeChange;
        }

        #region Recall With Invoke
        internal bool InvokeRequired => Dispatcher.Thread != Thread.CurrentThread;
        
        private bool RecallWithInvoke<T1, T2>(Func<T1, T2, bool> d, T1 c, T2 e)
        {
            try
            {
                try
                {
                    //return d.Invoke(c, e);
                    return (bool)Dispatcher.Invoke(d, c, e);
                }
                catch (NullReferenceException)
                {
                    // FIXME
                    return false;
                }
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        private void RecallWithInvokeA<T1, T2>(Action<T1, T2> d, T1 c, T2 e)
        {
            try
            {
                try
                {
                    //return d.Invoke(c, e);
                    Dispatcher.Invoke(d, c, e);
                }
                catch (NullReferenceException)
                {
                    // FIXME
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        #endregion

        #region IRC Events
        
        private void Channel_Closing(object sender, CancelEventArgs e)
        {
            var window = (FlexList)sender;
            var connection = window.Connection;
            var chan = window.WindowId;
            if(connection.Channels.Contains(chan))
                connection.SendLine("PART " + chan);
        }

        private bool Debug_OnLog(Connection c, LogEventArgs e)
        {
            if (!_specialWindows.ContainsKey("@@debug@@"))
                return true;

            var dbg = _specialWindows["@@debug@@"];
            dbg.AddLine((int)ColorTheme.Default, e.Text);

            return true;
        }
        private void Connections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Connection item in e.NewItems)
                    item.PropertyChanged += Connection_PropertyChanged;
            }
        }

        private void Connection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                RecallWithInvokeA(Connection_PropertyChanged, sender, e);
                return;
            }

            var conn = (Connection)sender;
            if (e.PropertyName == nameof(conn.State))
            {
                switch (conn.State)
                {
                    case ConnectionState.Connecting:
                        _connectionWindows[conn] = new Dictionary<string, FlexList>();

                        var window = CreateWindow(conn, "@@status@@",
                            $"Status [{conn.Server}:{conn.Port}]");
                        window.ShowTopic = false;
                        window.AddLine(0, "Connecting...");
                        window.Closed += (s, a) => window.Connection.Close();
                        window.ItemsSource = conn.Channels;
                        window.ListItemDoubleClickCommand = new RelayCommand(ConnectionListItem_DoubleClick);
                        break;
                    case ConnectionState.Connected:
                        //connection.SendLine("JOIN #asdftest");
                        break;
                    case ConnectionState.Disconnected:
                        // TODO
                        break;
                }
            }
        }

        private bool Connection_OnTopicText(Connection connection, CommandEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnTopicText, connection, e);
            }

            string target = e.Parameters[1];
            string text = e.Parameters[2];

            if (!ContainsWindow(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            this[connection, target].TopicText = text;
            this[connection, target].AddLine(12, $" * Topic is '{text}'.");

            return true;
        }

        private bool Connection_OnChannelTopic(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelTopic, connection, e);
            }

            string target = e.Target;
            string text = e.Text;
            var from = connection.Channels[target].GetDecoratedName(e.From);

            if (!ContainsWindow(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            this[connection, target].TopicText = text;
            this[connection, target].TopicInfo = $" * Topic set by {@from} on {DateTime.Now}.";
            this[connection, target].AddLine(12, Tools.TimeStamp(" * {0} changed Topic to '{1}'.", from, text));

            return true;
        }

        private bool Connection_OnTopicInfo(Connection connection, CommandEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnTopicInfo, connection, e);
            }

            var target = e.Parameters[1];
            var setBy = e.Parameters[2];
            var setDate = e.Parameters[3];

            if (!ContainsWindow(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            var date = new DateTime(1970, 1, 1).AddSeconds(int.Parse(setDate));

            var ln = $" * Topic set by {setBy} on {date}.";

            this[connection, target].TopicInfo = ln;
            this[connection, target].AddLine(12, ln);

            return true;
        }

        private bool Connection_OnChannelModes(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelModes, connection, e);
            }

            var t = Tools.TimeStamp(" * {0} changed modes: {1}", connection.Channels[e.Target].GetDecoratedName(e.From), e.Text);

            this[connection, e.Target].AddLine(7, (t));

            return true;
        }

        private bool Connection_OnJoin(Connection connection, TargetEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnJoin, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            if (e.From.Nickname != connection.Me.Nickname)
            {
                this[connection, e.Target].AddLine(9, Tools.TimeStamp(" *{0} joined the channel.", e.From));
            }

            return false;
        }

        private bool Connection_OnPart(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPart, connection, e);
            }

            if (e.From.Is(connection.Me))
            {
                ProcessChannelPart(connection, e);
            }
            else
            {
                if (!ContainsWindow(connection, e.Target))
                {
                    ProcessChannelJoin(connection, e);
                }

                string msg = Tools.TimeStamp(
                    string.IsNullOrEmpty(e.Text)
                        ? " * {0} left the channel"
                        : " * {0} left the channel ({1})", e.From, e.Text);

                this[connection, e.Target].AddLine(10, msg);
            }
            return false;
        }

        private bool Connection_OnKick(Connection connection, KickEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnKick, connection, e);
            }

            if (e.From.Is(connection.Me))
            {
                ProcessChannelPart(connection, e);
            }
            else
            {
                if (!ContainsWindow(connection, e.Target))
                {
                    ProcessChannelJoin(connection, e);
                }

                var msg = Tools.TimeStamp(
                    (e.Text.Length > 0)
                        ? " * {0} kicked {1} from the channel ({2})"
                        : " * {0} kicked {1} from the channel",
                    e.From.Nickname,
                    e.Who,
                    e.Text);

                this[connection, e.Target].AddLine(10, msg);
            }

            return false;
        }

        private bool Connection_OnQuit(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {

                return RecallWithInvoke(Connection_OnQuit, connection, e);
            }

            var msg = Tools.TimeStamp(" * {0} quit IRC ({1})", e.From.Nickname, e.Text);

            if (ContainsWindow(connection, e.From.Nickname))
            {
                this[connection, e.From.Nickname].AddLine(4, msg);
            }

            foreach (Channel ch in connection.Channels)
            {
                //e.From = ch.Users[e.From].User.Nickname;
                if (!ContainsWindow(connection, ch.Name))
                    continue;

                this[connection, ch.Name].AddLine(4, msg);
            }
            return false;
        }

        private bool Connection_OnNickChange(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnNickChange, connection, e);
            }

            var msg = Tools.TimeStamp(" * {0} changed nick to {1}", e.From.Nickname, e.Target);

            if (ContainsWindow(connection, e.From.Nickname))
            {
                var d = this[connection, e.From.Nickname];
                RemoveWindow(connection, e.From.Nickname);
                d.Title = e.Target;
                _connectionWindows[connection].Add(e.Target, d);
                this[connection, e.Target].AddLine(7, msg);
            }

            foreach (var ch in connection.Channels)
            {
                if (!ch.Users.Contains(e.Target) || !ContainsWindow(connection, ch.Name))
                    continue;

                var msg2 = Tools.TimeStamp(" * {0} changed nick to {1}", ch.GetDecoratedName(e.From), e.Target);

                this[connection, ch.Name].AddLine(7, msg2);
            }
            return false;
        }



        private bool Connection_OnChannelMessage(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelMessage, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = Tools.TimeStamp("({0}): {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            this[connection, e.Target].AddLine(15, msg);
            return false;
        }

        private bool Connection_OnChannelNotice(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelNotice, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = Tools.TimeStamp(" ** {0} **: {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            this[connection, e.Target].AddLine(11, msg);

            return false;
        }

        private bool Connection_OnChannelAction(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelAction, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = Tools.TimeStamp(" * {0} {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            this[connection, e.Target].AddLine(7, msg);
            return false;
        }

        private bool Connection_OnPrivateMessage(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateMessage, connection, e);
            }

            var wnd = OpenQueryWindow(connection, e.Target);

            var msg = Tools.TimeStamp("({0}): {1}", e.From.Nickname, e.Text);

            wnd.AddLine(15, msg);
            return false;
        }

        private bool Connection_OnPrivateNotice(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateNotice, connection, e);
            }

            if (e.Target == "AUTH")
            {
                this[connection, "@@status@@"].AddLine(11, Tools.TimeStamp(" ** AUTH **: {0}", e.Text));
            }
            else if (e.Target == "*")
            {
                this[connection, "@@status@@"].AddLine(11, Tools.TimeStamp(" -{0}- {1}", e.From, e.Text));
            }
            else
            {
                var wnd = OpenQueryWindow(connection, e.From.Nickname);

                var msg = Tools.TimeStamp(" ** {0} **: {1}", e.From.Nickname, e.Text);

                wnd.AddLine(11, msg);
            }
            return false;
        }

        private bool Connection_OnPrivateAction(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateAction, connection, e);
            }

            var wnd = OpenQueryWindow(connection, e.From.Nickname);

            var msg = Tools.TimeStamp(" * {0} {1}", e.From.Nickname, e.Text);

            wnd.AddLine(7, msg);
            return false;
        }

        private bool Connection_OnPrivateCtcpReply(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateCtcpReply, connection, e);
            }

            var msg = Tools.TimeStamp(" * [{0} CTCP {1} reply]: {2}", e.From.Nickname, e.Command, e.Text);
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.From.Nickname].AddLine(12, msg);
            }
            else
            {
                this[connection, "@@status@@"].AddLine(12, msg);
            }

            return false;
        }

        private bool Connection_OnPrivateCtcp(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateCtcp, connection, e);
            }

            if (e.Command == "ACTION")
                return false;

            var msg = Tools.TimeStamp(" * [{0} CTCP {1}]: {2}", e.From.Nickname, e.Command, e.Text);
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.From.Nickname].AddLine(12, msg);
            }
            else
            {
                this[connection, "@@status@@"].AddLine(12, msg);
            }

            return false;
        }

        private bool Connection_OnChannelCtcpReply(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelCtcpReply, connection, e);
            }

            var msg = Tools.TimeStamp(" * [{0} CTCP {1} reply]: {2}", e.From.Nickname, e.Command, e.Text);
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.Target].AddLine(12, msg);
            }
            else
            {
                this[connection, "@@status@@"].AddLine(12, msg);
            }

            return false;
        }

        private bool Connection_OnChannelCtcp(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelCtcp, connection, e);
            }

            if (e.Command == "ACTION")
                return false;

            var msg = Tools.TimeStamp(" * [{0} CTCP {1}]: {2}", e.From.Nickname, e.Command, e.Text);
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.Target].AddLine(12, msg);
            }
            else
            {
                this[connection, "@@status@@"].AddLine(12, msg);
            }

            return false;
        }

        private void ChannelListItem_DoubleClick(object obj)
        {
            var data = (Tuple<FlexList, object>)obj;
            var conn = data.Item1.Connection;
            var target = (ChannelUser) data.Item2;

            var window = OpenQueryWindow(conn, target.User.Nickname);

            DockingManager.ActiveContent = window.AnchorableParent;
        }

        private void ConnectionListItem_DoubleClick(object obj)
        {
            var data = (Tuple<FlexList, object>)obj;
            var conn = data.Item1.Connection;
            var target = (Channel)data.Item2;

            var window = this[conn, target.Name];

            DockingManager.ActiveContent = window.AnchorableParent;
        }

        private FlexList OpenQueryWindow(Connection connection, string target)
        {
            FlexList window;
            if (!TryGetWindow(connection, target, out window))
            {
                window = CreateWindow(connection, target, target);
                window.ShowTopic = false;
                window.ShowListbox = false;
            }
            return window;
        }

        private void ProcessChannelJoin(Connection connection, TargetEventArgs e)
        {
            if (!ContainsWindow(connection, e.Target))
            {
                var window = CreateWindow(connection, e.Target, e.Target);
                window.Closing += Channel_Closing;
                window.ItemsSource = connection.Channels[e.Target].Users;
                window.ListItemDoubleClickCommand = new RelayCommand(ChannelListItem_DoubleClick);
            }

            this[connection, e.Target].AddLine(0, Tools.TimeStamp("You Joined {0}.", e.Target));
        }

        private void ProcessChannelPart(Connection connection, MessageEventArgs e)
        {
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.Target].Close();
            }
        }

        #endregion
    }
}
