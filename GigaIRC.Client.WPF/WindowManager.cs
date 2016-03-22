using System;
using System.Collections.Generic;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Client.WPF.Tree;
using GigaIRC.Protocol;

namespace GigaIRC.Client.WPF
{
    internal class WindowManager
    {
        public Dictionary<Connection, WindowCollection> Connections { get; } = new Dictionary<Connection, WindowCollection>();

        public WindowCollection OtherWindows { get; } = new WindowCollection(null);

        public SessionData Data { get; } = new SessionData();

        public FlexList this[Connection cn, string wndName] => Connections[cn][wndName];
        public FlexList this[string wndName] => OtherWindows[wndName];

        public void Add(FlexList window)
        {
            switch (window.PanelType)
            {
                case PanelType.Status:
                    var data = new ConnectionData(window);
                    var conn = new WindowCollection(data);
                    Connections.Add(window.Connection, conn);
                    conn.Add(window);
                    Data.Items.Add(data);
                    break;
                case PanelType.Other:
                    OtherWindows.Add(window);
                    Data.OtherWindows.Add(window);
                    break;
                case PanelType.Channel:
                case PanelType.Query:
                    Connections[window.Connection].Add(window);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(FlexList window)
        {
            switch (window.PanelType)
            {
                case PanelType.Status:
                    WindowCollection data;
                    Connections.TryGetValue(window.Connection, out data);
                    Connections.Remove(window.Connection);
                    if(data != null)
                        Data.Items.Remove(data.Data);
                    break;
                case PanelType.Other:
                    OtherWindows.Remove(window);
                    Data.OtherWindows.Remove(window);
                    break;
                case PanelType.Channel:
                case PanelType.Query:
                    Connections[window.Connection].Remove(window);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool Contains(string id)
        {
            return OtherWindows.Contains(id);
        }

        public bool Contains(Connection cn, string id)
        {
            return Connections[cn].Contains(id);
        }

        public bool TryGetWindow(string id, out FlexList window)
        {
            return OtherWindows.TryGetWindow(id, out window);
        }

        public bool TryGetWindow(Connection cn, string id, out FlexList window)
        {
            return Connections[cn].TryGetWindow(id, out window);
        }

        public class WindowCollection
        {
            public Dictionary<string, FlexList> Windows { get; } = new Dictionary<string, FlexList>();

            public ConnectionData Data { get; }

            public FlexList this[string wndName] => Windows[wndName];

            public WindowCollection(ConnectionData data)
            {
                Data = data;
            }

            public void Add(FlexList window)
            {
                Windows.Add(window.WindowId, window);

                if (Data != null)
                {
                    switch (window.PanelType)
                    {
                        case PanelType.Other:
                            Data.OtherWindows.Add(window);
                            break;
                        case PanelType.Status:
                            Data.StatusWindow = window;
                            break;
                        case PanelType.Channel:
                            Data.ChannelWindows.Add(window);
                            break;
                        case PanelType.Query:
                            Data.QueryWindows.Add(window);
                            break;
                    }
                }
            }

            public void Remove(FlexList window)
            {
                Windows.Remove(window.WindowId);

                if (Data != null)
                {
                    switch (window.PanelType)
                    {
                        case PanelType.Other:
                            Data.OtherWindows.Remove(window);
                            break;
                        case PanelType.Status:
                            Data.StatusWindow = window;
                            break;
                        case PanelType.Channel:
                            Data.ChannelWindows.Remove(window);
                            break;
                        case PanelType.Query:
                            Data.QueryWindows.Remove(window);
                            break;
                    }
                }
            }

            public bool Contains(string id)
            {
                return Windows.ContainsKey(id);
            }

            public bool TryGetWindow(string id, out FlexList window)
            {
                return Windows.TryGetValue(id, out window);
            }
        }
    }
}