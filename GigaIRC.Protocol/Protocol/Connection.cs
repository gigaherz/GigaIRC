using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GigaIRC.Annotations;
using GigaIRC.Config;
using GigaIRC.Events;

namespace GigaIRC.Protocol
{
    public enum ConnectionState
    {
        Disconnected = 0,
        Connecting,
        Identifying,
        WaitingMOTD,
        Connected,
    }
        
    public class Connection: INotifyPropertyChanged
    {
        private ConnectionSocket _connectionSocket;

        public readonly Session Session;
        public readonly Server Server;
        public readonly Identity Identity;

        private bool _namesUpdateInProgress;
        private ConnectionState _state;
        private string _network;
        private bool _secure;
        private int _port;
        private string _address;
        private UserInfo _me;

        public UserList Users { get; } = new UserList();
        public ChannelCollection Channels { get; } = new ChannelCollection();
        public Dictionary<string, string> Features { get; } = new Dictionary<string, string>();
        public Capabilities Capabilities { get; }

        public Logger Logger { get; }

        public string Address
        {
            get { return _address; }
            private set
            {
                if (value == _address) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get { return _port; }
            private set
            {
                if (value == _port) return;
                _port = value;
                OnPropertyChanged();
            }
        }

        public bool Secure
        {
            get { return _secure; }
            private set
            {
                if (value == _secure) return;
                _secure = value;
                OnPropertyChanged();
            }
        }

        public string Network
        {
            get { return _network; }
            set
            {
                if (value == _network) return;
                _network = value;
                OnPropertyChanged();
            }
        }

        public ConnectionState State
        {
            get { return _state; }
            private set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged();
            }
        }

        public UserInfo Me
        {
            get { return _me; }
            private set
            {
                if (Equals(value, _me)) return;
                _me = value;
                OnPropertyChanged();
            }
        }

        public string PreModes { get; set; }
        public string PreChars { get; set; }
        public string ChanTypes { get; set; }

        internal Connection(Session sess, Server server, Identity id)
        {
            Capabilities = new Capabilities(this);

            Session = sess;
            Server = server;
            Identity = id;
            Secure = false;
            Network = server.Network?.Name;

            Logger = new Logger(this, server + ".log");

            State = ConnectionState.Disconnected;
        }

        private static string CtcpEscape(string n)
        {
            n = n.Replace("\x10", "\x10" + "\x10");
            n = n.Replace("\x0d", "\x10" + "r");
            n = n.Replace("\x0a", "\x10" + "n");
            n = n.Replace("\x00", "\x10" + "0");
            return n;
        }

        public async Task SendLineAsync(string line)
        {
            await _connectionSocket.SendLine(line);
        }

        public async void SendLine(string line)
        {
            await _connectionSocket.SendLine(line);
        }

        public void SendMessage(string dest, string line)
        {
            SendLine("PRIVMSG " + dest + " :" + line);
        }

        public void SendNotice(string dest, string line)
        {
            SendLine("NOTICE " + dest + " :" + line);
        }

        public void SendCTCP(string dest, string ctcpcmd)
        {
            SendMessage(dest, FormattingCodes.CTCP + CtcpEscape(ctcpcmd) + FormattingCodes.CTCP);
        }

        public void SendCTCPReply(string dest, string ctcpreply)
        {
            SendNotice(dest, FormattingCodes.CTCP + CtcpEscape(ctcpreply) + FormattingCodes.CTCP);
        }

#if false
        private void GetIPFromHttp()
        {
            Logger.LogLine(2, "Asking our ip to dyndns.org...");

            var getmyip = WebRequest.Create("http://checkip.dyndns.org:8245/");
            var givemyip = (HttpWebResponse)getmyip.GetResponse();

            Me = new UserInfo();

            var codeClass = (int) givemyip.StatusCode/100;

            if (codeClass != 2) 
                return;

            var readmyip = new StreamReader(givemyip.GetResponseStream());
            string ln;
            string ip = "";
            do
            {
                ln = readmyip.ReadLine();
                int p = ln.IndexOf("Current IP Address: ");
                if (p >= 0)
                {
                    ln = ln.Substring(p + "Current IP Address: ".Length);
                    string tch = ln.Substring(0, 1);
                    while ("0123456789.".IndexOf(tch) >= 0)
                    {
                        ln = ln.Substring(1);
                        ip += tch;
                        tch = ln.Substring(0, 1);
                    }
                }
            } while ((ip == "") && (!readmyip.EndOfStream));

            if (ip != "") 
                Me.IP = ip;
        }
#endif

        void CTCPCommandReceived(Command cmd)
        {
            var ctcp = new CTCPCommand(cmd);
            
            if (ChanTypes.Contains(ctcp.Target.Substring(0, 1)))
            {
                if (Session.OnChannelCtcp.Invoke(this, new CTCPEventArgs(cmd.From, ctcp)))
                    return;
            }
            else
            {
                if (Session.OnPrivateCtcp.Invoke(this, new CTCPEventArgs(cmd.From, ctcp)))
                    return;
            }

            if (ctcp.Is("PING"))
            {
                SendCTCPReply(cmd.From.Nickname, "PING " + ctcp.Text);
                Logger.LogLine(1, " * CTCP PING replied...");
            }
            else if (ctcp.Is("VERSION"))
            {
                SendCTCPReply(cmd.From.Nickname, "VERSION GigaIRC v0.1");
                Logger.LogLine(1, " * CTCP VERSION replied...");
            }
            else if (ctcp.Is("ACTION"))
            {
                // /me or /describe

                if (ChanTypes.Contains(ctcp.Target.Substring(0, 1)))
                {
                    Session.OnChannelAction.Invoke(this, new MessageEventArgs(cmd.From, ctcp.Target, ctcp.Text));
                }
                else
                {
                    Session.OnPrivateAction.Invoke(this, new MessageEventArgs(cmd.From, ctcp.Target, ctcp.Text));
                }
            }
        }

        void CTCPReplyReceived(Command cmd)
        {
            var ctcp = new CTCPCommand(cmd);

            if (ChanTypes.Contains(ctcp.Target.Substring(0, 1)))
            {
                Session.OnChannelCtcpReply.Invoke(this, new CTCPEventArgs(cmd.From, ctcp));
            }
            else
            {
                Session.OnPrivateCtcpReply.Invoke(this, new CTCPEventArgs(cmd.From, ctcp));
            }
        }

        void MessageReceived(Command cmd)
        {
            var chars = cmd.Params[1].ToCharArray();
            // if is a CTCP command
            if (chars.Length > 0 && chars[0] == '\x1')
            {
                CTCPCommandReceived(cmd);
            }
            else
            {
                //not a CTCP
                string target = cmd.Params[0];

                if (ChanTypes.Contains(target.Substring(0, 1)))
                {
                    Session.OnChannelMessage.Invoke(this, new MessageEventArgs(cmd.From, target, cmd.Params[1]));
                }
                else
                {
                    Session.OnPrivateMessage.Invoke(this, new MessageEventArgs(cmd.From, target, cmd.Params[1]));
                }
            }
        }

        void NoticeReceived(Command cmd)
        {
            // if is a CTCP reply
            if (cmd.Params[1].ToCharArray()[0] == '\x1')
            {
                CTCPReplyReceived(cmd);
            }
            else
            {
                //not a CTCP
                var target = cmd.Params[0];

                if (ChanTypes.Contains(target.Substring(0, 1)))
                {
                    Session.OnChannelNotice.Invoke(this, new MessageEventArgs(cmd.From, target, cmd.Params[1]));
                }
                else
                {
                    var args = new MessageEventArgs(cmd.From.Nickname == "" ? null : cmd.From,
                                                    target, cmd.Params[1]);
                    Session.OnPrivateNotice.Invoke(this, args);
                }
            }
        }

        internal void CommandReceived(Command cmd)
        {
            if (cmd.From.Nickname.Length > 0)
            {
                Users.Update(cmd.From);
            }

            if (Session.OnRawCommand.Invoke(this, new CommandEventArgs(cmd)))
            {
                // if handled, skip further processing
                return;
            }

            if (cmd.Is("PING"))
            {
                cmd.CmdText = "PONG";
                cmd.From = new UserInfo();
                SendLine(cmd.ToString());
                Logger.LogLine(2, " * PING? PONG!");
            }
            else if (cmd.Is("PRIVMSG"))
            {
                MessageReceived(cmd);
            }
            else if (cmd.Is("NOTICE"))
            {
                NoticeReceived(cmd);
            }
            else if (cmd.Is("CAP"))
            {
                Capabilities.Received(cmd);
            }
            else if (cmd.CmdText == "001") //Welcome message
            {
                if (Me == null) Me = new UserInfo();
                Me.Nickname = cmd.Params[0];
                Users.Add(Me);
                SendLine("USERHOST " + Me.Nickname);

                var line = cmd.Params.Last();
                if (line.StartsWith("Welcome to the ") && line.EndsWith(cmd.Params[0]))
                {
                    // Welcome to the <NETWORK> Internet Relay Chat Network <NICKNAME>
                    Network = line.Split(' ')[3];
                }
            }
            else if (cmd.CmdText == "005") //Features
            {
                int maxFeat = cmd.Params.Count - 1; // first param is the nick, last param is "are supported by this server"
                for (int i = 1; i < maxFeat; i++)
                {
                    string feature = cmd.Params[i];
                    string value = "";

                    int pos = feature.IndexOf('=');
                    if (pos >= 0)
                    {
                        value = feature.Substring(pos + 1);
                        feature = feature.Substring(0, pos);
                    }

                    Features[feature] = value;

                    switch (feature.ToUpper())
                    {

                        case "IRCX":
                            PreModes = "qov";
                            PreChars = ".@+";
                            SendLine("IRCX");
                            Logger.LogLine(1, "Server supports IRCX extensions, but they are not used.");
                            break;
                        case "CHANTYPES":
                            ChanTypes = value;
                            Logger.LogLine(1, "Server supports channel prefixes: {0}", value);
                            break;
                        case "PREFIX":
                            // Format of the value: (modes)prefixes
                            // each mode char is linked to a prefix char

                            pos = value.IndexOf(')');
                            PreChars = value.Substring(pos + 1);
                            PreModes = value.Substring(1, pos - 1);

                            Logger.LogLine(1, "Server supports prefixes: {0}", value);
                            break;
                        default:
                            if (value.Length > 0)
                                Logger.LogLine(1, "Server supports {0}: {1}", feature, value);
                            else
                                Logger.LogLine(1, "Server supports {0}.", feature);
                            break;
                    }
                }
            }
            else if (cmd.CmdText == "302")
            {
                //302 bomberbot :bomberbot=+ghzmomhome@83.56.254.177 
                int pos = cmd.Params[1].IndexOf('@');
                if (pos >= 0)
                {
                    if (Me.Address.Length == 0)
                        Me.Address = cmd.Params[1].Substring(pos + 1);
                }
            }
            else if ((cmd.CmdText == "376") //MOTD end
                || (cmd.CmdText == "422")) //NO MOTD
            {
                //assume we are connected
                Logger.LogLine(1, "Connection complete.");
                ChangeState(ConnectionState.Connected);
            }
            else if (cmd.CmdText == "353") //NAMES
            {
                //353 nick = chan :names
                var chan = Channels[cmd.Params[2]];

                var users = cmd.Params[3].Split(' ');
                foreach (var u in users)
                {
                    if (u.Length <= 0)
                        continue;

                    var prefixes = "";

                    int i = 0;
                    for (; ; i++)
                    {
                        if (PreChars.IndexOf(u[i]) < 0)
                            break;

                        prefixes += u[i];
                    }
                    var v = u.Substring(i);

                    Users.Update(new UserInfo(v));
                    var ui = Users[v];

                    chan.Users.Add(new ChannelUser(this, ui, prefixes));

                    if (!_namesUpdateInProgress)
                    {
                        _namesUpdateInProgress = true;
                        Session.OnChannelNamesUpdateStart.Invoke(this, new TextEventArgs(chan.Name));
                    }
                    Session.OnChannelNamesUpdateName.Invoke(this, new TargetEventArgs(ui, chan.Name));
                }
            }
            else if (cmd.CmdText == "366") //NAMES end
            {
                _namesUpdateInProgress = false;
                Session.OnChannelNamesUpdateEnd.Invoke(this, new TextEventArgs(cmd.Params[0]));
            }
            else if (cmd.Is("JOIN"))
            {
                Channel ch;
                if (cmd.From.Is(Me))
                {
                    ch = new Channel(cmd.Params[0]);
                    Channels.Add(ch);
                }
                else //the names list contains the client's own nick so no need to add it explicitly
                {
                    ch = Channels[cmd.Params[0]];

                    Users.Update(cmd.From);

                    ch.Users.Add(new ChannelUser(this, Users[cmd.From.Nickname], ""));

                    cmd.From.SeenIn.Add(ch);
                }

                Session.OnJoin.Invoke(this, new MessageEventArgs(cmd.From, cmd.Params[0], ""));

            }
            else if (cmd.Is("PART"))
            {
                if (cmd.From.Is(Me))
                {
                    Channels.Remove(cmd.Params[0]);
                }
                else
                {
                    Channel ch = Channels[cmd.Params[0]];
                    ch.Users.Remove(cmd.From.Nickname);
                    cmd.From.SeenIn.Remove(ch.Name);
                }

                var param = (cmd.Params.Count > 1) ? cmd.Params[1] : "";
                Session.OnPart.Invoke(this, new MessageEventArgs(cmd.From, cmd.Params[0], param));

                if (cmd.From.SeenIn != null && cmd.From.SeenIn.Count == 0)
                    Users.Remove(cmd.From.Nickname);
            }
            else if (cmd.Is("KICK"))
            {
                if (Me.Is(cmd.Params[1]))
                {
                    Channels.Remove(cmd.Params[0]);
                }
                else
                {
                    Channel ch = Channels[cmd.Params[0]];
                    ch.Users.Remove(cmd.Params[1]);
                    cmd.From.SeenIn.Remove(ch.Name);
                }

                Session.OnKick.Invoke(this, new KickEventArgs(cmd.From, cmd.Params[0], Users[cmd.Params[1]], cmd.Params[2]));

                if (cmd.From.SeenIn.Count == 0)
                    Users.Remove(cmd.From.Nickname);
            }
            else if (cmd.Is("QUIT"))
            {
                if (cmd.From.Is(Me))
                {
                    Channels.Clear();
                }
                else
                {
                    foreach (Channel ch in Channels)
                    {
                        ch.Users.Remove(cmd.From.Nickname);
                    }
                }

                var param = (cmd.Params.Count > 0) ? cmd.Params[0] : "";
                Session.OnQuit.Invoke(this, new MessageEventArgs(cmd.From, "", param));

                Users.Remove(cmd.From.Nickname);
            }
            else if (cmd.Is("MODE"))
            {
                string chanOrUser = cmd.Params[0];
                string chanChar = chanOrUser.Substring(0, 1);

                if (ChanTypes.Contains(chanChar))
                {
                    string modeString = string.Join(" ", cmd.Params.ToArray(), 1, cmd.Params.Count - 1);
                    Session.OnChannelModes.Invoke(this, new MessageEventArgs(cmd.From, chanOrUser, modeString));

                    string modechanges = cmd.Params[1];

                    var mods = new Queue<string>();

                    string addrem = "+";

                    while (modechanges.Length > 0)
                    {
                        string cur = modechanges.Substring(0, 1);
                        modechanges = modechanges.Substring(1);
                        if (cur == "+")
                        {
                            addrem = "+";
                        }
                        else if (cur == "-")
                        {
                            addrem = "-";
                        }
                        else
                        {
                            mods.Enqueue(addrem + cur);
                        }
                    }

                    int np = 2;

                    try
                    {
                        Channel ch = Channels[cmd.Params[0]];

                        while (mods.Count > 0)
                        {
                            string cur = mods.Dequeue();
                            addrem = cur.Substring(0, 1);
                            string mchar = cur.Substring(1);

                            if (PreModes.IndexOf(mchar, StringComparison.Ordinal) >= 0)
                            {
                                string mnick = cmd.Params[np++];

                                string pchar = PreChars.ToCharArray()[PreModes.IndexOf(mchar, StringComparison.Ordinal)].ToString();

                                string cpre = ch.Users[mnick].Prefix;

                                if (addrem == "+")
                                {
                                    if (cpre.IndexOf(pchar, StringComparison.Ordinal) < 0)
                                    {
                                        cpre += pchar;
                                    }
                                }
                                else
                                {
                                    cpre = cpre.Replace(pchar, "");
                                }
                                ch.Users[mnick].Prefix = cpre;

                                Logger.LogLine(1, "User " + mnick + " now has modes: " + cpre);
                            }
                            else
                            {
                                //TODO: handle chanmodes with arguments
                                if (addrem == "+")
                                {
                                    ch.Modes.Update(mchar[0], "");
                                }
                                else
                                {
                                    ch.Modes.Remove(mchar[0]);
                                }
                            }

                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        //ignore
                    }
                }
                else
                {
                    //TODO: user modes
                }
            }
            else if (cmd.Is("NICK"))
            {
                if (cmd.From.Is(Me))
                {
                    Me.Nickname = cmd.Params[0];
                }
                else
                {
                    UserInfo u = Users[cmd.From.Nickname];
                    u.Nickname = cmd.Params[0];
                }
                Session.OnNickChange.Invoke(this, new MessageEventArgs(cmd.From, cmd.Params[0], ""));
            }
            else if (cmd.Is("TOPIC"))
            {
                if (cmd.From.Is(Me))
                {
                    Me.Nickname = cmd.Params[0];
                }
                else
                {
                    UserInfo u = Users[cmd.From.Nickname];
                    u.Nickname = cmd.Params[0];
                }
                Session.OnChannelTopic.Invoke(this, new MessageEventArgs(cmd.From, cmd.Params[0], cmd.Params[1]));
            }
        }

        public void Connect()
        {
            _connectionSocket = new ConnectionSocket(this);
            _connectionSocket.Connect();

            PreModes = "ov";
            PreChars = "@+";
            ChanTypes = "#";
        }

        public void Close()
        {
            _connectionSocket.Close();
            Logger.LogLine(2, "Connection closed.");
        }

        internal void ChangeState(ConnectionState newS)
        {
            State = newS;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void UpdateRemote(IPEndPoint ep)
        {
            Address = ep.Address.ToString();
            Port = ep.Port;

            Dns.BeginGetHostEntry(ep.Address, GetHostEntryCallback, null);
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                Address = Dns.EndGetHostEntry(ar).HostName;
            }
            catch (Exception e)
            {
                Logger.LogLine(3, "ERROR: Querying address for '{0}': {1}", Address, e);
            }
        }
    }
}
