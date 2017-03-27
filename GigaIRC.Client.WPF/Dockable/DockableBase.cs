using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using GigaIRC.Client.WPF.Annotations;
using Xceed.Wpf.AvalonDock.Layout;

namespace GigaIRC.Client.WPF.Dockable
{
    public abstract class DockableBase : UserControl, INotifyPropertyChanged
    {
        private string _title;
        private Brush _designBackground;
        private PanelType _panelType = PanelType.Other;
        private bool _isExpanded = true;

        public LayoutContent LayoutParent { get; set; }

        // For the treeview only
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public PanelType PanelType
        {
            get { return _panelType; }
            set
            {
                if (value == _panelType) return;
                _panelType = value;
                OnPropertyChanged();
            }
        }

        public Brush DesignBackground
        {
            get { return _designBackground; }
            set
            {
                if (Equals(value, _designBackground)) return;
                _designBackground = value;
                OnPropertyChanged();
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    Background = value;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool OnClosing()
        {
            var args = new CancelEventArgs();
            Closing?.Invoke(this, args);
            return args.Cancel;
        }

        public void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public event CancelEventHandler Closing;
        public event EventHandler Closed;
    }
}
