
using System;
using System.Windows;
using System.Windows.Controls;
using GigaIRC.Client.WPF.Tree;

namespace GigaIRC.Client.WPF.Dockable
{
    public partial class SessionTree
    {
        private SessionData _root;
        private DockableBase _selectedWindow;

        public DataTemplateSelector TreeItemsTemplateSelector { get; set; }

        public class DefaultDataTemplateSelector : DataTemplateSelector
        {
            private readonly SessionTree _parent;

            public DefaultDataTemplateSelector(SessionTree parent)
            {
                this._parent = parent;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is SessionData)
                    return (DataTemplate)_parent.FindResource("SessionDataTemplate");
                if (item is ConnectionData)
                    return (DataTemplate)_parent.FindResource("ConnectionDataTemplate");
                if (item is GenericItem)
                    return (DataTemplate)_parent.FindResource("GenericItemDataTemplate");
                if (item is DockableBase)
                    return (DataTemplate)_parent.FindResource("WindowDataTemplate");
                return null;
            }
        }

        public SessionData Root
        {
            get { return _root; }
            set
            {
                if (Equals(value, _root)) return;
                _root = value;
                OnPropertyChanged();
            }
        }

        public DockableBase SelectedWindow
        {
            get { return _selectedWindow; }
            set
            {
                if (Equals(value, _selectedWindow)) return;
                _selectedWindow = value;
                OnPropertyChanged();
            }
        }

        public SessionTree()
        {
            TreeItemsTemplateSelector = new DefaultDataTemplateSelector(this);

            InitializeComponent();

            Title = "Session";
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var status = e.NewValue as ConnectionData;
            if (status != null)
                SelectedWindow = status.StatusWindow;
            else
                SelectedWindow = e.NewValue as DockableBase;
        }
    }
}
