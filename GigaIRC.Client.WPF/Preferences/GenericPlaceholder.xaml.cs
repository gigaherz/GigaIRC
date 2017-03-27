using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GigaIRC.Client.WPF.Preferences
{
    public partial class GenericPlaceholder
    {
        public RelayCommand ActivatePageCommand => new RelayCommand(p => ActivatePage(p as PreferencesTree));
        private void ActivatePage(PreferencesTree node)
        {
            node.IsSelected = true;
            while (node.Parent != null)
            {
                node = node.Parent;
                node.IsExpanded = true;
            }
        }

        public GenericPlaceholder()
        {
            InitializeComponent();
        }
    }
}
