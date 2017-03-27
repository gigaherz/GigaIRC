using System;
using System.Windows.Input;

namespace GigaIRC.Client.WPF.Dockable
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser
    {
        public WebBrowser()
        {
            InitializeComponent();
        }

        public RelayCommand GoCommand => new RelayCommand(_ => Go());
        void Go()
        {
            var addr = Address.Text;
            var uri = addr.Contains(":") ? new Uri(addr) : new Uri("http://" + addr);
            Browser.Navigate(uri);
        }

        public RelayCommand BackCommand => new RelayCommand(_ => Back());
        void Back()
        {
            Browser.GoBack();
        }

        public RelayCommand ForwardCommand => new RelayCommand(_ => Forward());
        void Forward()
        {
            Browser.GoForward();
        }

        public RelayCommand RefreshCommand => new RelayCommand(_ => Refresh());
        void Refresh()
        {
            Browser.Refresh();
        }

        private void Address_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Go();
                e.Handled = true;
            }
        }

        private void Browser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Browser.DocumentTitle))
            {
                Title = string.Format("Browser: {0}",
                    string.IsNullOrEmpty(Browser.DocumentTitle) 
                        ? Browser.Source?.ToString() ?? "Blank Page"
                        : Browser.DocumentTitle);
            }
            else if (e.PropertyName == nameof(Browser.Source))
            {
                Address.Text = Browser.Source?.ToString();
            }
        }
    }
}
