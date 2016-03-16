using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GigaIRC.Client.Dockable;
using GigaIRC.Config;
using GigaIRC.Events;
using GigaIRC.Protocol;
using GigaIRC.Util;
using WeifenLuo.WinFormsUI.Docking;

namespace GigaIRC.Client
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        private readonly Dictionary<Connection,Dictionary<string, ListWindow>> dockedWindows = new Dictionary<Connection, Dictionary<string, ListWindow>>();
        private readonly Dictionary<string, ListWindow> dockedWindows2 = new Dictionary<string, ListWindow>();

        public ListWindow this[Connection cn, string wndName]
        {
            get
            {
                var dict = (cn == null) ? dockedWindows2 : dockedWindows[cn];
                return dict[wndName];
            }
            private set
            {
                var dict = (cn == null) ? dockedWindows2 : dockedWindows[cn];
                dict[wndName] = value;
            }
        }

        public readonly Session Session = new Session();

        public MainForm()
        {
            Instance = this;

            InitializeComponent();

            Session.Settings.LoadFromFile(@"settings.cfg");

            // NOTE: If you add an event here, remember to detach it below!

            Session.OnConnectionStateChange += Connection_OnStateChange;

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
            Session.OnChannelNamesUpdateStart += Connection_OnNamesUpdateStart;
            Session.OnChannelNamesUpdateName += Connection_OnNamesUpdate;
            //Session.OnChannelNamesUpdateEnd += Connection_OnNamesUpdateEnd;
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

            // NOTE: If you add an event here, remember to detach it below!
        }

        private void DetachEvents()
        {
            Session.OnConnectionStateChange -= Connection_OnStateChange;

            Session.OnRawCommand.Remove(Connection_OnTopicText);
            Session.OnRawCommand.Remove(Connection_OnTopicInfo);

            Session.OnJoin -= Connection_OnJoin;
            Session.OnPart -= Connection_OnPart;
            Session.OnQuit -= Connection_OnQuit;
            Session.OnKick -= Connection_OnKick;

            Session.OnNickChange -= Connection_OnNickChange;

            Session.OnChannelMessage -= Connection_OnChannelMessage;
            Session.OnChannelNotice -= Connection_OnChannelNotice;
            Session.OnChannelAction -= Connection_OnChannelAction;
            Session.OnChannelTopic -= Connection_OnChannelTopic;
            Session.OnChannelNamesUpdateStart -= Connection_OnNamesUpdateStart;
            Session.OnChannelNamesUpdateName -= Connection_OnNamesUpdate;
            Session.OnChannelCtcp -= Connection_OnChannelCtcp;
            Session.OnChannelCtcpReply -= Connection_OnChannelCtcpReply;


            Session.OnPrivateMessage -= Connection_OnPrivateMessage;
            Session.OnPrivateNotice -= Connection_OnPrivateNotice;
            Session.OnPrivateAction -= Connection_OnPrivateAction;
            Session.OnPrivateCtcp -= Connection_OnPrivateCtcp;
            Session.OnPrivateCtcpReply -= Connection_OnPrivateCtcpReply;

            Session.OnChannelModes -= Connection_OnChannelModes;

            Session.OnLogLine -= Debug_OnLog;
        }

        #region Form Events
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var opts = new ConnectOptions
                           {
                               FullName = "GigaIRC User",
                               Username = "GigaIRC",
                               Nickname = "GigaIRC",
                               AltNickname = "GigaIRC2",
                               Server = "efnet.xs4all.nl",
                               Port = "6667"
                           };
            if (opts.ShowDialog() == DialogResult.Cancel)
                return;

            var id = new Identity
                         {
                             FullName = opts.FullName,
                             Username = opts.Username,
                             Nickname = opts.Nickname,
                             AltNickname = opts.AltNickname,
                         };

            var svr = new Server(new Network {Name = "Test"})
                          {
                              Address = opts.Server
                          };
            svr.PortRanges.Add(new Tuple<int, int>(6667, 6667));

            Session.ConnectTo(svr, id);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var window = AddDockableWindow(null, "@@debug@@", "Debug Window");
            window.ShowTitlePanel = false;
            window.ShowListbox = false;
            window.Show(dockPanel1);
            window.AddLine(0, "Program Initialized.");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetachEvents();
            Session.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var window = new PreferencesWindow { TabText = "Preferences" };

            switch (dockPanel1.DocumentStyle)
            {
                case DocumentStyle.DockingMdi:
                    window.MdiParent = this;
                    window.Show(dockPanel1);
                    break;
                case DocumentStyle.SystemMdi:
                    window.MdiParent = this;
                    window.Show();
                    break;
                default:
                    window.Show(dockPanel1);
                    break;
            }
        }

        private void newBrowserTab_Click(object sender, EventArgs e)
        {
            var window = new Dockable.WebBrowserWindow {TabText = "about:blank"};

            switch (dockPanel1.DocumentStyle)
            {
                case DocumentStyle.DockingMdi:
                    window.MdiParent = this;
                    window.Show(dockPanel1);
                    break;
                case DocumentStyle.SystemMdi:
                    window.MdiParent = this;
                    window.Show();
                    break;
                default:
                    window.Show(dockPanel1);
                    break;
            }
        }
        #endregion

        #region Window Management
        private ListWindow AddDockableWindow(Connection cn, string id, string title)
        {
            var window = new ListWindow(new Tuple<Connection, string>(cn, id));
            window.OnInput += Window_OnInput;

            this[cn, id] = window;

            switch (dockPanel1.DocumentStyle)
            {
                case DocumentStyle.DockingMdi:
                    window.MdiParent = this;
                    window.Show(dockPanel1);
                    break;
                case DocumentStyle.SystemMdi:
                    window.MdiParent = this;
                    window.Show();
                    break;
                default:
                    window.Show(dockPanel1);
                    break;
            }

            window.Text = title;

            return window;
        }
        
        private void RemoveWindow(Connection cn, string id)
        {
            var dict = (cn == null) ? dockedWindows2 : dockedWindows[cn];
            dict.Remove(id);
        }

        private bool ContainsWindow(Connection cn, string id)
        {
            var dict = (cn == null) ? dockedWindows2 : dockedWindows[cn];
            return dict.ContainsKey(id);
        }
        #endregion


        #region Input Handler

        private void Window_OnInput(object sender, OnInputEventArgs e)
        {
            var d = (ListWindow)sender;
            var connection = d.Connection;
            var windowId = d.WindowId;

            if (e.Text.Substring(0, 1) == "/")
            {
                //parse command
                var t = e.Text.Substring(1);
                var cmd = t.Split(' ');

                if (String.Compare(cmd[0], "me", true) == 0)
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
                else if(connection != null)
                {
                    connection.SendLine(t);
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

        #region Recall With Invoke
        private bool RecallWithInvoke<T1, T2>(Func<T1, T2, bool> d, T1 c, T2 e)
            where T2 : EventArgs
        {
            try
            {
                try
                {
                    //return d.Invoke(c, e);
                    return (bool) Invoke(d, c, e);
                }
                catch(NullReferenceException)
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

        //private Func<T1, T2, bool> AutoInvokeFunc<T1, T2>(Func<T1, T2, bool> fn)
        //    where T2: EventArgs
        //{
        //    return (p1, p2) => InvokeRequired ? RecallWithInvoke(fn, p1, p2) : fn(p1, p2);
        //}
        #endregion

        #region IRC Events

        private bool Debug_OnLog(Connection c, LogEventArgs e)
        {
            if (dockedWindows2.ContainsKey("@@debug@@"))
            {
                var dbg = dockedWindows2["@@debug@@"];
                dbg.AddLine((int)ColorTheme.Default, e.Text);
            }

            return true;
        }
        
        private void Channel_FormClosing(object sender, FormClosingEventArgs e)
        {
            var window = (ListWindow)sender;
            var connection = window.Connection;
            var chan = window.WindowId;

            RemoveWindow(connection, chan);
            this[connection, "@@status@@"].Listbox.Items.Remove(chan);
            connection.SendLine("PART " + chan);
        }

        private bool Connection_OnStateChange(Connection connection, ConnectionStateEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnStateChange, connection, e);
            }

            switch (e.NewState)
            {
                case ConnectionState.Connecting:
                    dockedWindows[connection] = new Dictionary<string, ListWindow>();

                    var window = AddDockableWindow(connection, "@@status@@",
                        $"Status [{connection.Server}:{connection.Port}]");
                    window.ShowTitlePanel = false;
                    window.Show(dockPanel1);
                    window.AddLine(0, "Connecting...");
                    break;
                case ConnectionState.Connected:
                    //connection.SendLine("JOIN #asdftest");
                    break;
                case ConnectionState.Disconnected:
                    break;
            }

            return true;
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
                ProcessChannelJoin(connection, new MessageEventArgs(null,target, ""));
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

            var date = new DateTime(1970,1,1).AddSeconds(int.Parse(setDate));

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

        private bool Connection_OnNamesUpdateStart(Connection connection, TextEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnNamesUpdateStart, connection, e);
            }

            if (ContainsWindow(connection, e.Text))
            {
                var list = this[connection, e.Text].Listbox;

                list.Items.Clear();
                list.Sorted = true;
            }

            return false;
        }

        private bool Connection_OnNamesUpdate(Connection connection, TargetEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnNamesUpdate, connection, e);
            }

            if (ContainsWindow(connection, e.Target))
            {
                var list = this[connection, e.Target].Listbox;

                list.Items.Add(connection.Channels[e.Target].Users[e.From.Nickname]);
            }
            
            return false;
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

                this[connection, e.Target].Listbox.Items.Add(connection.Channels[e.Target].Users[e.From.Nickname]);
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

                foreach (ChannelUser item in this[connection, e.Target].Listbox.Items)
                {
                    if (e.From.Is(item.User))
                        continue;

                    this[connection, e.Target].Listbox.Items.Remove(item);
                    break;
                }
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

                foreach (ChannelUser item in this[connection, e.Target].Listbox.Items)
                {
                    if (!e.From.Is(item.User)) 
                        continue;

                    this[connection, e.Target].Listbox.Items.Remove(item);
                    break;
                }
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
                foreach (ChannelUser item in this[connection, ch.Name].Listbox.Items)
                {
                    if (!e.From.Is(item.User))
                        continue;

                    this[connection, ch.Name].Listbox.Items.Remove(item);
                    break;
                }
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
                d.Text = e.Target;
                dockedWindows[connection].Add(e.Target, d);
                this[connection, e.Target].AddLine(7, msg);
            }

            foreach (var ch in connection.Channels)
            {
                if (!ch.Users.Contains(e.Target) || !ContainsWindow(connection, ch.Name)) 
                    continue;

                var msg2 = Tools.TimeStamp(" * {0} changed nick to {1}", ch.GetDecoratedName(e.From), e.Target);

                this[connection, ch.Name].AddLine(7, msg2);
                this[connection, ch.Name].Listbox.Items.Clear();
                this[connection, ch.Name].Listbox.Items.AddRange(ch.Users.ToArray());
                this[connection, ch.Name].Listbox.Refresh();
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

            this[connection, e.Target].AddLine(7, msg );
            return false;
        }

        private bool Connection_OnPrivateMessage(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateMessage, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                var window = AddDockableWindow(connection, e.Target, e.Target);
                window.ShowListbox = false;
            }

            var msg = Tools.TimeStamp("({0}): {1}", e.From.Nickname, e.Text);

            this[connection, e.Target].AddLine(15, msg);
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
                if (!ContainsWindow(connection, e.Target))
                {
                    AddDockableWindow(connection, e.Target, e.Target);
                }

                var msg = Tools.TimeStamp(" ** {0} **: {1}", e.From.Nickname, e.Text);

                this[connection, e.Target].AddLine(11, msg);
            }
            return false;
        }

        private bool Connection_OnPrivateAction(Connection connection, MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                return RecallWithInvoke(Connection_OnPrivateAction, connection, e);
            }

            if (!ContainsWindow(connection, e.Target))
            {
                AddDockableWindow(connection, e.Target, e.Target);
            }

            var msg = Tools.TimeStamp(" * {0} {1}", e.From.Nickname, e.Text);

            this[connection, e.Target].AddLine(7, msg);
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
                this[connection, e.Target].AddLine(12, msg);
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
                this[connection, e.Target].AddLine(12, msg);
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

        private void ProcessChannelJoin(Connection connection, TargetEventArgs e)
        {
            if (!ContainsWindow(connection, e.Target))
            {

                this[connection, "@@status@@"].Listbox.Items.Add(e.Target);

                var window = AddDockableWindow(connection, e.Target, e.Target);
                window.FormClosing += Channel_FormClosing;
            }

            this[connection, e.Target].AddLine(0, Tools.TimeStamp("You Joined {0}.", e.Target));
        }

        private void ProcessChannelPart(Connection connection, MessageEventArgs e)
        {
            if (ContainsWindow(connection, e.Target))
            {
                this[connection, e.Target].Hide();
                this[connection, e.Target].Close();
            }
            RemoveWindow(connection, e.Target);
            this[connection, "@@status@@"].Listbox.Items.Remove(e.Target);
        }

        #endregion

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Session.Settings.ImportMircServersIni(@"H:\mIRC\servers.ini");
            Session.Settings.SaveToFile(@"settings.cfg");
        }
        
    }
}
