using System;
using System.Collections.Generic;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Client.WPF.Tree;
using GigaIRC.Protocol;

namespace GigaIRC.Client.WPF
{
    public class WindowManager
    {
        public Dictionary<Connection, WindowCollection> Connections { get; } = new Dictionary<Connection, WindowCollection>();

        public WindowCollection OtherWindows { get; } = new WindowCollection(null);

        public SessionData Data { get; } = new SessionData();

        public FlexList this[Connection cn, string wndName] => Connections[cn][wndName];
        public FlexList this[string wndName] => OtherWindows[wndName];

        public void Add(DockableBase window)
        {
            switch (window.PanelType)
            {
                case PanelType.Status:
                    var status = (FlexList)window;
                    var data = new ConnectionData(status);
                    var conn = new WindowCollection(data);
                    Connections.Add(status.Connection, conn);
                    conn.Add(window);
                    Data.Items.Add(data);
                    break;
                case PanelType.Other:
                    OtherWindows.Add(window);
                    Data.OtherWindows.Add(window);
                    break;
                case PanelType.Channel:
                case PanelType.Query:
                    var target = (FlexList)window;
                    Connections[target.Connection].Add(target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(DockableBase window)
        {
            switch (window.PanelType)
            {
                case PanelType.Status:
                    var status = (FlexList)window;
                    WindowCollection data;
                    Connections.TryGetValue(status.Connection, out data);
                    Connections.Remove(status.Connection);
                    if(data != null)
                        Data.Items.Remove(data.Data);
                    break;
                case PanelType.Other:
                    OtherWindows.Remove(window);
                    Data.OtherWindows.Remove(window);
                    break;
                case PanelType.Channel:
                case PanelType.Query:
                    var target = (FlexList)window;
                    Connections[target.Connection].Remove(window);
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
            window = null;
            return Connections.TryGetValue(cn, out var connection) && Connections[cn].TryGetWindow(id, out window);
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

            public void Add(DockableBase window)
            {
                var flex = window as FlexList;
                if (flex != null)
                    Windows.Add(flex.WindowId, flex);

                if (Data != null)
                {
                    switch (window.PanelType)
                    {
                        case PanelType.Other:
                            Data.OtherWindows.Add(window);
                            break;
                        case PanelType.Status:
                            Data.StatusWindow = (FlexList)window;
                            break;
                        case PanelType.Channel:
                            Data.ChannelWindows.Add((FlexList)window);
                            break;
                        case PanelType.Query:
                            Data.QueryWindows.Add((FlexList)window);
                            break;
                    }
                }
            }

            public void Remove(DockableBase window)
            {
                var flex = window as FlexList;
                if (flex != null)
                    Windows.Remove(flex.WindowId);

                if (Data != null)
                {
                    switch (window.PanelType)
                    {
                        case PanelType.Other:
                            Data.OtherWindows.Remove(window);
                            break;
                        case PanelType.Status:
                            Data.StatusWindow = (FlexList)window;
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