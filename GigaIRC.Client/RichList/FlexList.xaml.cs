using System.Collections.ObjectModel;
using GigaIRC.Util;

namespace GigaIRC.RichList
{
    /// <summary>
    /// Interaction logic for FlexList.xaml
    /// </summary>
    public partial class FlexList
    {
        public readonly ObservableCollection<LineInfo> Lines = new ObservableCollection<LineInfo>();

        public FlexList()
        {
            InitializeComponent();
        }
    }
}
