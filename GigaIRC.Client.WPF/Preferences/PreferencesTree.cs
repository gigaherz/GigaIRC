using GigaIRC.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GigaIRC.Client.WPF.Preferences
{
    public class PreferencesTree : INotifyPropertyChanged
    {
        public static PreferencesTree Instance { get; } = new PreferencesBuilder()
            .Category("IRC")
                .Configuration("Identities", new Identities())
                .Configuration("Networks & Servers", new Networks())
            .EndCategory()
            .Build();

        public ObservableCollection<PreferencesTree> Children { get; } = new ObservableCollection<PreferencesTree>();

        public PreferencesTree Parent { get; private set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Equals(value, _name)) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (Equals(value, _isSelected)) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (Equals(value, _isExpanded)) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        private PreferencesPage _preferencesPage;
        public PreferencesPage PreferencesPage
        {
            get { return _preferencesPage; }
            set
            {
                if (ReferenceEquals(value, _preferencesPage)) return;
                _preferencesPage = value;
                OnPropertyChanged();
            }
        }

        public PreferencesTree(PreferencesTree parent)
        {
            Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class NodeBuilder
        {
            protected readonly PreferencesTree node;

            protected NodeBuilder(PreferencesTree node)
            {
                this.node = node;
            }

            protected PreferencesTree CreateNode(string name, PreferencesPage page)
            {
                if (page == null)
                    page = new GenericPlaceholder();
                var child = new PreferencesTree(node) { Name = name, PreferencesPage = page };
                node.Children.Add(child);
                if (page != null)
                    page.Node = child;
                return child;
            }
        }

        private class CategoryBuilder<TParent> : NodeBuilder
        {
            private readonly TParent builder;

            internal CategoryBuilder(TParent builder, PreferencesTree node)
                : base(node)
            {
                this.builder = builder;
            }

            public TParent EndCategory()
            {
                return builder;
            }

            public CategoryBuilder<CategoryBuilder<TParent>> Category(string name, PreferencesPage page = null)
            {
                var child = CreateNode(name, page);
                return new CategoryBuilder<CategoryBuilder<TParent>>(this, child);
            }

            internal CategoryBuilder<TParent> Configuration(string name, PreferencesPage page = null)
            {
                CreateNode(name, page);
                return this;
            }
        }

        private class PreferencesBuilder : NodeBuilder
        {
            public PreferencesBuilder()
                : base(new PreferencesTree(null))
            {
            }

            public PreferencesTree Build()
            {
                return node;
            }

            public CategoryBuilder<PreferencesBuilder> Category(string name, PreferencesPage page = null)
            {
                var child = CreateNode(name, page);
                return new CategoryBuilder<PreferencesBuilder>(this, child);
            }
        }
    }
}
