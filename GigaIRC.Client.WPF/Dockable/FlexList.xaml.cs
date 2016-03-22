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
        private object _itemsSource;
        private PanelType _panelType = PanelType.Other;

        public ICommand ListItemDoubleClickCommand { get; set; }
        public ICommand LinkClickedCommand { get; set; }

        public DataTemplateSelector ListItemsTemplateSelector { get; set; }

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
                OnPropertyChanged(nameof(TopicVisibility));
            }
        }
        public Visibility TopicVisibility => _showTopic ? Visibility.Visible : Visibility.Collapsed;

        public bool ShowListbox
        {
            get { return _showListbox; }
            set
            {
                if (value == _showListbox) return;
                _showListbox = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListboxVisibility));
            }
        }
        public Visibility ListboxVisibility => _showTopic ? Visibility.Visible : Visibility.Collapsed;

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
        public Visibility InputVisibility => _showInput ? Visibility.Visible : Visibility.Collapsed;

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
        }

        private void OnLinkClicked(object obj)
        {
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

        public void AddLine(int color, string text)
        {
            if (_content == null)
                return;

            var line = new LineInfo((ColorCode)color, text);
            Lines.Add(line);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _content.Blocks.Add(LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked)));

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
                    _content.Blocks.Add(LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked)));
                }
                else
                {
                    var after = _content.Blocks.ElementAt(before);
                    _content.Blocks.InsertBefore(after, LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked)));
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
                LineToParagraphConverter.ToParagraph(line, new RelayCommand(OnLinkClicked), (Paragraph)at);
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
        }

        public void Close()
        {
            var anchorable = AnchorableParent;
            anchorable?.Close();
        }

        private void ItemsListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListItemDoubleClickCommand != null && ListItemDoubleClickCommand.CanExecute(ItemsListBox.SelectedItem))
            {
                var data = Tuple.Create(this, ItemsListBox.SelectedItem);
                ListItemDoubleClickCommand.Execute(data);
            }
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
