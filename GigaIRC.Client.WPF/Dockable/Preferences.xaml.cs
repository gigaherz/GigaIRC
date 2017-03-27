using GigaIRC.Client.WPF.Preferences;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GigaIRC.Client.WPF.Dockable
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : INotifyPropertyChanged
    {
        private PreferencesPage _currentPage;
        public PreferencesPage CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                if (ReferenceEquals(_currentPage, value)) return;
                _currentPage = value;
                //Tree.selec = _currentPage.Node;
                OnPropertyChanged();
            }
        }

        public Preferences()
        {
            InitializeComponent();
        }

        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Tree.SelectedValue is PreferencesTree node)
            {
                if (node.PreferencesPage != null && node.PreferencesPage != CurrentPage)
                {
                    CurrentPage = node.PreferencesPage;
                }
            }
        }
    }
}
