using System.Collections.ObjectModel;
using System.ComponentModel;
using GigaIRC.Client.WPF.Dockable;
using GigaIRC.Protocol;

namespace GigaIRC.Client.WPF.Tree
{
    public class ConnectionData : GenericItem
    {
        public readonly Connection Connection;

        public FlexList StatusWindow {get; set;}
        public ObservableCollection<object> ChannelWindows { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> QueryWindows { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> OtherWindows { get; } = new ObservableCollection<object>();

        public ConnectionData(FlexList statusWindow)
        {
            StatusWindow = statusWindow;
            Connection = statusWindow.Connection;

            DisplayName = StatusWindow.Title;
            StatusWindow.PropertyChanged += Connection_PropertyChanged;

            Items.Add(new GenericItem { DisplayName = "Channel", Items = ChannelWindows });
            Items.Add(new GenericItem { DisplayName = "Query", Items = QueryWindows });
            Items.Add(new GenericItem { DisplayName = "Other", Items = OtherWindows });
        }

        private void Connection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DisplayName = StatusWindow.Title;
        }
    }
}