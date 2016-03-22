using System.Collections.ObjectModel;

namespace GigaIRC.Client.WPF.Tree
{
    public class SessionData : GenericItem
    {
        public ObservableCollection<object> OtherWindows { get; } = new ObservableCollection<object>();

        public SessionData()
        {
            Items.Add(new GenericItem { DisplayName = "Other", Items = OtherWindows });
        }
    }
}
