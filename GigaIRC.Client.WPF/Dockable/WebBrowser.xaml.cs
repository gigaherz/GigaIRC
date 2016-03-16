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
                if (!string.IsNullOrEmpty(Browser.DocumentTitle))
                {
                    Title = Browser.DocumentTitle;
                }
                else
                {
                    Title =  Browser.Source?.ToString() ?? "Blank Page";
                }
            }
            else if (e.PropertyName == nameof(Browser.Source))
            {
                Address.Text = Browser.Source?.ToString();
            }
        }
    }
}
