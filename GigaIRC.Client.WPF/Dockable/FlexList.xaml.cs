using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using GigaIRC.Client.WPF.Util;
using GigaIRC.Protocol;
using GigaIRC.Util;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Diagnostics;

namespace GigaIRC.Client.WPF.Dockable
{
    public partial class FlexList
    {
        public readonly ObservableCollection<LineInfo> Lines = new ObservableCollection<LineInfo>();
        public ObservableCollection<object> ListItems { get; } = new ObservableCollection<object>();

        private readonly FlowDocument _content;
        private string _topicText;
        private string _topicInfo;
        private bool _listSorted;
        private bool _showTopic = true;
        private bool _showListbox = true;
        private bool _showInput = true;
        private string _topicTextModifiable;
        private bool _showTopicInfo = false;
        private object _itemsSource;
        private GridLength _listWidth = new GridLength(120);
        private Regex _nicknamesRegex;
        private bool _matchListItemsInText = false;

        public ICommand ListItemDoubleClickCommand { get; set; }
        public ICommand LinkClickedCommand { get; set; }

        public DataTemplateSelector ListItemsTemplateSelector { get; set; }

        public bool MatchListItemsInText
        {
            get { return _matchListItemsInText; }
            set
            {
                _matchListItemsInText = value;
                if (!value)
                    _nicknamesRegex = null;
                else
                    ListItems_CollectionChanged(null, null);
            }
        }

        public class DefaultDataTemplateSelector : DataTemplateSelector
        {
            private readonly FlexList _parent;

            public DefaultDataTemplateSelector(FlexList parent)
            {
                _parent = parent;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is ChannelUser)
                    return (DataTemplate)_parent.FindResource("ChannelUserDataTemplate");
                if (item is Channel)
                    return (DataTemplate)_parent.FindResource("ChannelDataTemplate");
                return null;
            }
        }

        public GridLength ListWidth
        {
            get { return _showListbox ? _listWidth : new GridLength(); }
            set
            {
                if (value.Equals(_listWidth)) return;
                _listWidth = value;
                OnPropertyChanged();
            }
        }

        public object ItemsSource
        {
            get { return _itemsSource; }
            set
            {
                if (Equals(value, _itemsSource)) return;
                _itemsSource = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTopic
        {
            get { return _showTopic; }
            set
            {
                if (value == _showTopic) return;
                _showTopic = value;
                OnPropertyChanged();
            }
        }

        public bool ShowListbox
        {
            get { return _showListbox; }
            set
            {
                if (value == _showListbox) return;
                _showListbox = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListWidth));
            }
        }

        public bool ShowInput
        {
            get { return _showInput; }
            set
            {
                if (value == _showInput) return;
                _showInput = value;
                OnPropertyChanged();
            }
        }

        public bool ShowInputbox
        {
            get
            {
                return ShowInput && IsFocused;
            }
        }

        public event TextInputEventHandler OnInput;

        public Connection Connection { get; set; }

        public string WindowId { get; set; }

        public string TopicText
        {
            get { return _topicText; }
            set
            {
                if (value == _topicText) return;
                _topicText = value;
                OnPropertyChanged();
                TopicTextModifiable = value;
            }
        }

        public string TopicTextModifiable
        {
            get { return _topicTextModifiable; }
            set
            {
                if (value == _topicTextModifiable) return;
                _topicTextModifiable = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowSetTopicButton));
            }
        }

        public bool ShowSetTopicButton
        {
            get
            {
                return ShowTopic && (TopicText != TopicTextModifiable);
            }
        }

        public string TopicInfo
        {
            get { return _topicInfo; }
            set
            {
                if (value == _topicInfo) return;
                _topicInfo = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTopicInfo
        {
            get
            {
                return _showTopicInfo;
            }
            set
            {
                if (value == _showTopicInfo) return;
                _showTopicInfo = value;
                OnPropertyChanged();
            }
        }

        public bool ListSorted
        {
            get { return _listSorted; }
            set
            {
                if (value == _listSorted) return;
                _listSorted = value;
                OnPropertyChanged();
            }
        }

        public FlexList()
        {
            _itemsSource = ListItems;
            ListItemsTemplateSelector = new DefaultDataTemplateSelector(this);

            InitializeComponent();

            ItemsListBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));

            _content = MainContent.Document;
            _content.Blocks.Clear();

            MainContent.IsDocumentEnabled = true;

            _nicknamesRegex = null;
            ListItems.CollectionChanged += ListItems_CollectionChanged;
        }

        private void ListItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_matchListItemsInText)
                _nicknamesRegex = new Regex("\b(" + string.Join("|", ListItems) + ")\b", RegexOptions.IgnoreCase);
        }

        private void OnLinkClicked(object obj)
        {
            if (obj is Uri uri && uri.Scheme == "list-item-select")
            {
                Debug.WriteLine(uri);
                return;
            }

            if (LinkClickedCommand != null && LinkClickedCommand.CanExecute(obj))
            {
                LinkClickedCommand.Execute(obj);
            }
        }

        // IRCList interface implementation
        public void Clear()
        {
            Lines.Clear();

            _content?.Blocks.Clear();
        }

        public void AddLine(ColorCode color, string text)
        {
            if (_content == null)
                return;

            var line = new LineInfo(color, text);
            Lines.Add(line);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _content.Blocks.Add(LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked), _nicknamesRegex));

                //if(shouldScroll)
                MainContent.ScrollToEnd();
            }));
        }

        public void AddLine(int before, int color, string text)
        {
            if (_content == null)
                return;

            var line = new LineInfo((ColorCode)color, text);
            Lines.Insert(before, line);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (before == _content.Blocks.Count)
                {
                    _content.Blocks.Add(LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked), _nicknamesRegex));
                }
                else
                {
                    var after = _content.Blocks.ElementAt(before);
                    _content.Blocks.InsertBefore(after, LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked), _nicknamesRegex));
                }
            }));
        }

        public void SetLine(int number, int color, string text)
        {
            if (_content == null)
                return;

            var line = new LineInfo((ColorCode)color, text);
            Lines[number] = line;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var at = _content.Blocks.ElementAt(number);
                LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked), _nicknamesRegex, (Paragraph)at);
            }));
        }

        public void RemoveLine(int number)
        {
            Lines.RemoveAt(number);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _content.Blocks.Remove(_content.Blocks.ElementAt(number));
            }));
        }

        public string GetLine(int number)
        {
            return Lines[number].Line;
        }

        public int GetLineColor(int number)
        {
            return (int)Lines[number].Color;
        }

        public int LineCount()
        {
            return Lines.Count;
        }

        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                OnInput?.Invoke(this, new TextInputEventArgs(Input.Text));
                Input.Text = null;
            }
            else if (e.Key == Key.Tab)
            {

            }
        }

        public void Close()
        {
            var anchorable = LayoutParent;
            anchorable?.Close();
        }

        private void ItemsListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListItemDoubleClickCommand != null &&
                ListItemDoubleClickCommand.CanExecute(ItemsListBox.SelectedItem))
            {
                ListItemDoubleClickCommand.Execute(Tuple.Create(this, ItemsListBox.SelectedItem));
            }
        }

        // Annoying, but IsKeyboardFocusWithin is not a dependency property so I can't bind to it directly.
        private void TextBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowTopicInfo = TopicTextBox.IsKeyboardFocusWithin;
        }

        private void MainContent_KeyDown(object sender, KeyEventArgs e)
        {
            Input.Focus();
        }
    }

    public class TextInputEventArgs : EventArgs
    {
        public string Text;

        public TextInputEventArgs(string text)
        {
            Text = text;
        }
    }

    public delegate void TextInputEventHandler(object sender, TextInputEventArgs e);
}
