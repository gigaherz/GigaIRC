using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GigaIRC.Config;
using GigaIRC.Events;

namespace GigaIRC.Protocol
{
    public class Session
    {
        public ObservableCollection<Connection> Connections { get; } = new ObservableCollection<Connection>();

        public Settings Settings { get; } = new Settings();

        public EventChain<Connection, LogEventArgs> OnLogLine = new EventChain<Connection, LogEventArgs>();
        
        public FilteringEventChain<Connection, CommandEventArgs> OnRawCommand = new FilteringEventChain<Connection, CommandEventArgs>();

        public EventChain<Connection, TargetEventArgs> OnJoin = new EventChain<Connection, TargetEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnPart = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnQuit = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, KickEventArgs> OnKick = new EventChain<Connection, KickEventArgs>();
        
        public EventChain<Connection, MessageEventArgs> OnNickChange = new EventChain<Connection, MessageEventArgs>();
        
        public EventChain<Connection, MessageEventArgs> OnChannelTopic = new EventChain<Connection, MessageEventArgs>();

        public EventChain<Connection, TextEventArgs> OnChannelNamesUpdateStart = new EventChain<Connection, TextEventArgs>();
        public EventChain<Connection, TargetEventArgs> OnChannelNamesUpdateName = new EventChain<Connection, TargetEventArgs>();
        public EventChain<Connection, TextEventArgs> OnChannelNamesUpdateEnd = new EventChain<Connection, TextEventArgs>();
        
        public EventChain<Connection, MessageEventArgs> OnChannelMessage = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnChannelNotice = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnChannelAction = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, CTCPEventArgs> OnChannelCtcp = new EventChain<Connection, CTCPEventArgs>();
        public EventChain<Connection, CTCPEventArgs> OnChannelCtcpReply = new EventChain<Connection, CTCPEventArgs>();
        
        public EventChain<Connection, MessageEventArgs> OnPrivateMessage = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnPrivateNotice = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnPrivateAction = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, CTCPEventArgs> OnPrivateCtcp = new EventChain<Connection, CTCPEventArgs>();
        public EventChain<Connection, CTCPEventArgs> OnPrivateCtcpReply = new EventChain<Connection, CTCPEventArgs>();
        
        public EventChain<Connection, ModeChangeEventArgs> OnUserModeChange = new EventChain<Connection, ModeChangeEventArgs>();
        public EventChain<Connection, ModeChangeEventArgs> OnChannelModeChange = new EventChain<Connection, ModeChangeEventArgs>();

        public EventChain<Connection, MessageEventArgs> OnUserModes = new EventChain<Connection, MessageEventArgs>();
        public EventChain<Connection, MessageEventArgs> OnChannelModes = new EventChain<Connection, MessageEventArgs>();

        public void ConnectTo(Server svr)
        {
            if (svr.Network.DefaultIdentity != null)
                ConnectTo(svr, svr.Network.DefaultIdentity);
            else if (Settings.DefaultIdentity != null)
                ConnectTo(svr, Settings.DefaultIdentity);
            else
                throw new InvalidOperationException("No identity specified and no default identity configured");
        }

        public void ConnectTo(Server svr, Identity id)
        {
            var cn = new Connection(this, svr, id);
            Connections.Add(cn);
            cn.Connect();
        }

        public void Close()
        {
            foreach(var cn in Connections)
            {
                cn.Close();
            }
        }
    }
}
