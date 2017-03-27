using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using GigaIRC.Client.WPF.Completion;
using GigaIRC.Client.WPF.Dialogs;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Client.WPF.Util;
using GigaIRC.Config;
using GigaIRC.Events;
using GigaIRC.Protocol;
using Xceed.Wpf.AvalonDock.Layout;
using WebBrowser = GigaIRC.Client.WPF.Dockable.WebBrowser;

namespace GigaIRC.Client.WPF
{
    public partial class MainWindow
    {
        private Dockable.Preferences _preferences;
        private SessionTree _treeList;

        private LayoutAnchorablePane _sidePaneLeft;
        private LayoutAnchorablePane _sidePaneRight;

        public static MainWindow Instance { get; private set; }

        public Session Session { get; } = new Session();

        private readonly WindowManager _windows = new WindowManager();

        public WindowManager WindowManager => _windows;

        private readonly CommandHandler _commandHandler = new CommandHandler();

        public CommandHandler CommandHandler => _commandHandler;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            ShowDebug();
            ShowTreeList();

            Session.Settings.LoadFromFile(@"settings.cfg");

            AttachEvents();

            _windows["@@debug@@"].AddLine(0, "Program Initialization Finished.");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var conn in Session.Connections.ToList())
                conn.Close();
        }

        private void LinkClicked(object obj)
        {
            NewBrowser(obj as Uri);
        }

        private void AttachContent(DockableBase panel)
        {
            var document = new LayoutDocument
            {
                Content = panel,
                Title = panel.Title
            };

            panel.LayoutParent = document;

            panel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Title")
                    panel.LayoutParent.Title = panel.Title;
            };

            LayoutDocumentPane parent = FindAttachmentPoint();
            parent.Children.Add(document);

            _windows.Add(panel);
            panel.Closed += (sender, args) => _windows.Remove(panel);
        }

        private LayoutDocumentPane FindAttachmentPoint()
        {
            var active = DockingManager.ActiveContent;
            if (active is DockableBase dockable)
            {
                var parent = dockable.LayoutParent.FindParent<LayoutDocumentPane>();
                if (parent != null)
                    return parent;
            }

            if (MasterPanel.ContainsChildOfType<LayoutDocumentPane>())
            {
                if (MasterPanel.Children.OfType<LayoutDocumentPaneGroup>().Any())
                {
                    var g = MasterPanel.Children.OfType<LayoutDocumentPaneGroup>().First();
                    while (g.Children.OfType<LayoutDocumentPaneGroup>().Any())
                    {
                        g = g.Children.OfType<LayoutDocumentPaneGroup>().First();
                    }
                    return g.Children.OfType<LayoutDocumentPane>().First();
                }
                else
                {
                    return MasterPanel.Children.OfType<LayoutDocumentPane>().First();
                }
            }
            else
            {
                var c = MasterPanel;
                while (c.ChildrenCount > 0 && c.ContainsChildOfType<LayoutPanel>())
                {
                    c = c.Children.OfType<LayoutPanel>().First();
                }
                var l = new LayoutDocumentPane();
                c.Children.Add(l);
                return l;
            }
        }

        private void AttachSide(DockableBase panel, bool right)
        {
            var anchorable = new LayoutAnchorable
            {
                Content = panel,
                Title = panel.Title
            };

            panel.LayoutParent = anchorable;

            panel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Title")
                    panel.LayoutParent.Title = panel.Title;
            };

            anchorable.Closed += (sender, args) => panel.OnClosed();
            anchorable.Closing += (sender, args) => args.Cancel = panel.OnClosing();

            LayoutAnchorablePane pane;
            if (right)
            {
                if (_sidePaneRight == null)
                {
                    _sidePaneRight = new LayoutAnchorablePane { DockWidth = new GridLength(200) };
                    MasterPanel.Children.Add(_sidePaneRight);
                }
                pane = _sidePaneRight;
            }
            else
            {
                if (_sidePaneLeft == null)
                {
                    _sidePaneLeft = new LayoutAnchorablePane { DockWidth = new GridLength(200) };
                    MasterPanel.Children.Insert(0, _sidePaneLeft);
                }
                pane = _sidePaneLeft;
            }

            pane.Children.Add(anchorable);
        }

        public RelayCommand OpenPreferencesCommand => new RelayCommand(_ => OpenPreferences());
        private void OpenPreferences()
        {
            if (_preferences == null)
            {
                _preferences = new Dockable.Preferences();
                _preferences.Closed += (sender, args) => _preferences = null;

                AttachContent(_preferences);
            }
            DockingManager.ActiveContent = _preferences;
        }

        public RelayCommand ShowDebugCommand => new RelayCommand(_ => ShowDebug());
        private void ShowDebug()
        {
            FlexList wnd;

            if (!_windows.TryGetWindow("@@debug@@", out wnd))
            {
                var debugPanel = CreateWindow("@@debug@@", "Debug", PanelType.Other);
                debugPanel.ShowTopic = false;
                debugPanel.ShowListbox = false;
                debugPanel.LinkClickedCommand = new RelayCommand(LinkClicked);
                debugPanel.OnInput += (sender, args) => debugPanel.AddLine(ColorTheme.Default, args.Text);
            }

            DockingManager.ActiveContent = wnd;
        }

        public RelayCommand ShowTreeListCommand => new RelayCommand(_ => ShowTreeList());
        private void ShowTreeList()
        {
            if (_treeList == null)
            {
                _treeList = new SessionTree {Root = _windows.Data};
                _treeList.Closed += (sender, args) => _treeList = null;
                _treeList.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(_treeList.SelectedWindow))
                    {
                        if (_treeList.SelectedWindow != null)
                        {
                            DockingManager.ActiveContent = _treeList.SelectedWindow;
                        }
                    }
                };

                AttachSide(_treeList, false);
            }
            DockingManager.ActiveContent = _treeList;
        }

        public RelayCommand NewBrowserCommand => new RelayCommand(_ => NewBrowser());
        private void NewBrowser(Uri url = null)
        {
            var browser = new WebBrowser();
            AttachContent(browser);

            if(url != null)
                browser.Browser.Source = url;
            
            DockingManager.ActiveContent = browser;
        }

        public RelayCommand NewConnectionCommand => new RelayCommand(_ => NewConnection());
        private void NewConnection()
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

                svr.PortRangeCollection.Add(new Tuple<int, int>(6667, 6667));

                Session.ConnectTo(svr, id);
            }
        }

        public RelayCommand ExitCommand => new RelayCommand(_ => Exit());
        private void Exit()
        {
            Close();
        }

        #region Window Management
        private FlexList CreateWindow(string id, string title, PanelType panelType)
        {
            return CreateWindow(null, id, title, panelType);
        }

        private FlexList CreateWindow(Connection cn, string id, string title, PanelType panelType)
        {
            var panel = new FlexList
            {
                Connection = cn,
                WindowId = id,
                PanelType = panelType,
                Title = title,
                LinkClickedCommand = new RelayCommand(LinkClicked)
            };
            panel.OnInput += Chat_OnInput;

            AttachContent(panel);

            return panel;
        }

        #endregion

        #region Input Handler
        private void Chat_OnInput(object sender, TextInputEventArgs e)
        {
            _commandHandler.ProcessCommand((FlexList)sender, e.Text);
        }
        #endregion

        public void AttachEvents()
        {
            // NOTE: If you add an event here, remember to detach it below!

            Session.Connections.CollectionChanged += Connections_CollectionChanged;

            Session.OnRawCommand.Add(Connection_OnTopicText, "332");
            Session.OnRawCommand.Add(Connection_OnTopicInfo, "333");

            Session.OnRawCommand.Add(Connection_OnRawStatus, 
                "001", "002", "003", "004", "005", "302", "372", "375", "376");

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
            Session.OnUserModes += Connection_OnUserModes;

            Session.OnLogLine += Debug_OnLog;

            // TODO
            //Session.OnChannelModeChange += Connection_OnChannelModeChange;
            //Session.OnUserModeChange += Connection_OnUserModes;
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
            if (!_windows.Contains("@@debug@@"))
                return true;

            var dbg = _windows["@@debug@@"];
            dbg.AddLine(ColorTheme.Default, e.Text);

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
                FlexList window;
                switch (conn.State)
                {
                    case ConnectionState.Connecting:
                        window = CreateWindow(conn, "@@status@@",
                            $"Status {conn.Network} [{conn.Address}:{conn.Port}]",
                            PanelType.Status);
                        window.ShowTopic = false;
                        window.AddLine(0, "Connecting...");
                        window.Closed += (s, a) =>
                        {
                            _windows.Remove(window);
                            window.Connection.Close();
                        };
                        window.ItemsSource = conn.Channels;
                        window.ListItemDoubleClickCommand = new RelayCommand(ConnectionListItem_DoubleClick);
                        break;
                    case ConnectionState.Connected:
                        if (_windows.TryGetWindow(conn, "@@status@@", out window))
                        {
                            window.AddLine(0, "Connection completed.");
                        }
                        // TODO: Autojoin
                        break;
                    case ConnectionState.Disconnected:
                        if (_windows.TryGetWindow(conn, "@@status@@", out window))
                        {
                            window.AddLine(0, "Disconnected.");
                        }
                        break;
                }
            }
            else
            {
                var window = _windows[conn, "@@status@@"];
                if (window != null)
                {
                    window.Title = $"Status {conn.Network} [{conn.Address}:{conn.Port}]";
                }
            }
        }

        private bool Connection_OnRawStatus(Connection c, CommandEventArgs e)
        {
            var status = _windows[c, "@@status@@"];

            status.AddLine(ColorTheme.Default, TimeStamp.Format("{0}", e.Parameters.Last()));

            return false;
        }

        private bool Connection_OnTopicText(Connection connection, CommandEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnTopicText, connection, e);
            }

            string target = e.Parameters[1];
            string text = e.Parameters[2];

            if (!_windows.Contains(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            _windows[connection, target].TopicText = text;
            _windows[connection, target].AddLine(ColorTheme.Topic, $" * Topic is '{text}'.");

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

            if (!_windows.Contains(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            _windows[connection, target].TopicText = text;
            _windows[connection, target].TopicInfo = $" * Topic set by {@from} on {DateTime.Now}.";
            _windows[connection, target].AddLine(ColorTheme.Topic, TimeStamp.Format(" * {0} changed Topic to '{1}'.", from, text));

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

            if (!_windows.Contains(connection, target))
            {
                ProcessChannelJoin(connection, new MessageEventArgs(null, target, ""));
            }

            var date = new DateTime(1970, 1, 1).AddSeconds(int.Parse(setDate));

            var ln = $" * Topic set by {setBy} on {date}.";

            _windows[connection, target].TopicInfo = ln;
            _windows[connection, target].AddLine(ColorTheme.TopicInfo, ln);

            return true;
        }

        private bool Connection_OnUserModes(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnUserModes, connection, e);
            }

            var t = TimeStamp.Format(" * {0} changed modes: {1}", e.From, e.Text);

            _windows[connection, "@@status@@"].AddLine(ColorTheme.Mode, t);

            return true;
        }

        private bool Connection_OnChannelModes(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelModes, connection, e);
            }

            var t = TimeStamp.Format(" * {0} changed modes: {1}", connection.Channels[e.Target].GetDecoratedName(e.From), e.Text);

            _windows[connection, e.Target].AddLine(ColorTheme.Mode, t);

            return true;
        }

        private bool Connection_OnJoin(Connection connection, TargetEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnJoin, connection, e);
            }

            if (!_windows.Contains(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            if (e.From.Nickname != connection.Me.Nickname)
            {
                _windows[connection, e.Target].AddLine(ColorTheme.Join, TimeStamp.Format(" *{0} joined the channel.", e.From));
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
                if (!_windows.Contains(connection, e.Target))
                {
                    ProcessChannelJoin(connection, e);
                }

                string msg = TimeStamp.Format(
                    string.IsNullOrEmpty(e.Text)
                        ? " * {0} left the channel"
                        : " * {0} left the channel ({1})", e.From, e.Text);

                _windows[connection, e.Target].AddLine(ColorTheme.Part, msg);
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
                if (!_windows.Contains(connection, e.Target))
                {
                    ProcessChannelJoin(connection, e);
                }

                var msg = TimeStamp.Format(
                    (e.Text.Length > 0)
                        ? " * {0} kicked {1} from the channel ({2})"
                        : " * {0} kicked {1} from the channel",
                    e.From.Nickname,
                    e.Who,
                    e.Text);

                _windows[connection, e.Target].AddLine(ColorTheme.Kick, msg);
            }

            return false;
        }

        private bool Connection_OnQuit(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {

                return RecallWithInvoke(Connection_OnQuit, connection, e);
            }

            var msg = TimeStamp.Format(" * {0} quit IRC ({1})", e.From.Nickname, e.Text);

            if (_windows.Contains(connection, e.From.Nickname))
            {
                _windows[connection, e.From.Nickname].AddLine(ColorTheme.Quit, msg);
            }

            foreach (var ch in connection.Channels)
            {
                //e.From = ch.Users[e.From].User.Nickname;
                if (!_windows.Contains(connection, ch.Name))
                    continue;

                _windows[connection, ch.Name].AddLine(ColorTheme.Quit, msg);
            }
            return false;
        }

        private bool Connection_OnNickChange(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnNickChange, connection, e);
            }

            var msg = TimeStamp.Format(" * {0} changed nick to {1}", e.From.Nickname, e.Target);

            if (_windows.Contains(connection, e.From.Nickname))
            {
                var d = _windows[connection, e.From.Nickname];
                _windows.Remove(d);
                d.Title = e.Target;
                _windows.Add(d);
                _windows[connection, e.Target].AddLine(ColorTheme.Nick, msg);
            }

            foreach (var ch in connection.Channels)
            {
                if (!ch.Users.Contains(e.Target) || !_windows.Contains(connection, ch.Name))
                    continue;

                var msg2 = TimeStamp.Format(" * {0} changed nick to {1}", ch.GetDecoratedName(e.From), e.Target);

                _windows[connection, ch.Name].AddLine(ColorTheme.Nick, msg2);
            }
            return false;
        }

        internal bool Connection_OnChannelMessage(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelMessage, connection, e);
            }

            if (!_windows.Contains(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = TimeStamp.Format("({0}): {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            var color = e.From.Is(connection.Me) ? ColorTheme.TextOwn : ColorTheme.TextOthers;

            _windows[connection, e.Target].AddLine(color, msg);
            return false;
        }

        private bool Connection_OnChannelNotice(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelNotice, connection, e);
            }

            if (!_windows.Contains(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = TimeStamp.Format(" ** {0} **: {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            var color = e.From.Is(connection.Me) ? ColorTheme.NoticeOwn : ColorTheme.NoticeOthers;

            _windows[connection, e.Target].AddLine(color, msg);

            return false;
        }

        internal bool Connection_OnChannelAction(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelAction, connection, e);
            }

            if (!_windows.Contains(connection, e.Target))
            {
                ProcessChannelJoin(connection, e);
            }

            var msg = TimeStamp.Format(" * {0} {1}",
                                      connection.Channels[e.Target].GetDecoratedName(e.From),
                                      e.Text);

            var color = e.From.Is(connection.Me) ? ColorTheme.ActionOwn : ColorTheme.ActionOthers;

            _windows[connection, e.Target].AddLine(color, msg);
            return false;
        }

        internal bool Connection_OnPrivateMessage(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateMessage, connection, e);
            }

            var wnd = OpenQueryWindow(connection, e.Target);

            var msg = TimeStamp.Format("({0}): {1}", e.From.Nickname, e.Text);

            var color = e.From.Is(connection.Me) ? ColorTheme.TextOwn : ColorTheme.TextOthers;

            wnd.AddLine(color, msg);
            return false;
        }

        private bool Connection_OnPrivateNotice(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateNotice, connection, e);
            }

            var color = e.From.Is(connection.Me) ? ColorTheme.NoticeOwn : ColorTheme.NoticeOthers;

            var status = _windows[connection, "@@status@@"];
            if (e.Target == "AUTH")
            {
                status.AddLine(color, TimeStamp.Format(" ** AUTH **: {0}", e.Text));
            }
            else if (e.Target == "*")
            {
                status.AddLine(color, TimeStamp.Format(" -{0}- {1}", e.From, e.Text));
            }
            else
            {
                status.AddLine(color, TimeStamp.Format(" ** {0} **: {1}", e.From.Nickname, e.Text));
            }

            FlexList wnd;
            if (_windows.TryGetWindow(connection, e.From.Nickname, out wnd))
            {
                wnd.AddLine(color, TimeStamp.Format(" ** {0} **: {1}", e.From.Nickname, e.Text));
            }

            return false;
        }

        internal bool Connection_OnPrivateAction(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateAction, connection, e);
            }

            var wnd = OpenQueryWindow(connection, e.From.Nickname);

            var msg = TimeStamp.Format(" * {0} {1}", e.From.Nickname, e.Text);

            var color = e.From.Is(connection.Me) ? ColorTheme.ActionOwn : ColorTheme.ActionOthers;

            wnd.AddLine(color, msg);
            return false;
        }

        private bool Connection_OnPrivateCtcpReply(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateCtcpReply, connection, e);
            }

            var msg = TimeStamp.Format(" * [{0} CTCP {1} reply]: {2}", e.From.Nickname, e.Command, e.Text);
            if (_windows.Contains(connection, e.Target))
            {
                _windows[connection, e.From.Nickname].AddLine(ColorTheme.CtcpReply, msg);
            }
            else
            {
                _windows[connection, "@@status@@"].AddLine(ColorTheme.CtcpReply, msg);
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

            var msg = TimeStamp.Format(" * [{0} CTCP {1}]: {2}", e.From.Nickname, e.Command, e.Text);
            if (_windows.Contains(connection, e.Target))
            {
                _windows[connection, e.From.Nickname].AddLine(ColorTheme.Ctcp, msg);
            }
            else
            {
                _windows[connection, "@@status@@"].AddLine(ColorTheme.Ctcp, msg);
            }

            return false;
        }

        private bool Connection_OnChannelCtcpReply(Connection connection, CTCPEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnChannelCtcpReply, connection, e);
            }

            var msg = TimeStamp.Format(" * [{0} CTCP {1} reply]: {2}", e.From.Nickname, e.Command, e.Text);
            if (_windows.Contains(connection, e.Target))
            {
                _windows[connection, e.Target].AddLine(ColorTheme.CtcpReply, msg);
            }
            else
            {
                _windows[connection, "@@status@@"].AddLine(ColorTheme.CtcpReply, msg);
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

            var msg = TimeStamp.Format(" * [{0} CTCP {1}]: {2}", e.From.Nickname, e.Command, e.Text);
            if (_windows.Contains(connection, e.Target))
            {
                _windows[connection, e.Target].AddLine(ColorTheme.Ctcp, msg);
            }
            else
            {
                _windows[connection, "@@status@@"].AddLine(ColorTheme.Ctcp, msg);
            }

            return false;
        }

        private void ChannelListItem_DoubleClick(object obj)
        {
            var data = (Tuple<FlexList, object>)obj;
            var conn = data.Item1.Connection;
            var target = (ChannelUser) data.Item2;

            var window = OpenQueryWindow(conn, target.User.Nickname);

            DockingManager.ActiveContent = window;
        }

        private void ConnectionListItem_DoubleClick(object obj)
        {
            var data = (Tuple<FlexList, object>)obj;
            var conn = data.Item1.Connection;
            var target = (Channel)data.Item2;

            var window = _windows[conn, target.Name];

            DockingManager.ActiveContent = window;
        }

        private FlexList OpenQueryWindow(Connection connection, string target)
        {
            FlexList window;
            if (!_windows.TryGetWindow(connection, target, out window))
            {
                window = CreateWindow(connection, target, target, PanelType.Query);
                window.ShowTopic = false;
                window.ShowListbox = false;
            }
            return window;
        }

        private void ProcessChannelJoin(Connection connection, TargetEventArgs e)
        {
            if (!_windows.Contains(connection, e.Target))
            {
                var window = CreateWindow(connection, e.Target, e.Target, PanelType.Channel);
                window.Closing += Channel_Closing;
                window.ItemsSource = connection.Channels[e.Target].Users;
                window.ListItemDoubleClickCommand = new RelayCommand(ChannelListItem_DoubleClick);
            }

            _windows[connection, e.Target].AddLine(0, TimeStamp.Format("You Joined {0}.", e.Target));
        }

        private void ProcessChannelPart(Connection connection, MessageEventArgs e)
        {
            if (_windows.Contains(connection, e.Target))
            {
                _windows[connection, e.Target].Close();
            }
        }

        #endregion

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            //_windows.SetActive(DockingManager.ActiveContent);
        }

        private void DockingManager_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            ((DockableBase)e.Document.Content).OnClosed();
        }

        private void DockingManager_DocumentClosing(object sender, Xceed.Wpf.AvalonDock.DocumentClosingEventArgs e)
        {
            ((DockableBase)e.Document.Content).OnClosing();
        }
    }
}
