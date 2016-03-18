using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GigaIRC.Client.WPF.Annotations;

namespace GigaIRC.Client.WPF.Util
{
    public class FormsWebBrowser : System.Windows.Forms.Integration.WindowsFormsHost, INotifyPropertyChanged
    {
        readonly System.Windows.Forms.WebBrowser _browser =
            new System.Windows.Forms.WebBrowser();

        public Uri Source
        {
            get { return _browser.Url; }
            set { _browser.Url = value; }
        }

        public string DocumentTitle => _browser.DocumentTitle;

        public FormsWebBrowser()
        {
            Child = _browser;
            _browser.DocumentTitleChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(DocumentTitle));
            };

            _browser.Navigating += (sender, e) =>
            {
                OnPropertyChanged(nameof(Source));
            };

            _browser.Navigated += (sender, e) =>
            {
                OnPropertyChanged(nameof(Source));
            };

            _browser.DocumentCompleted += (sender, e) =>
            {
                OnPropertyChanged(nameof(Source));
            };
        }

        public void Navigate(Uri uri)
        {
            _browser.Navigate(uri);
        }

        public void GoBack()
        {
            _browser.GoBack();
        }

        public void GoForward()
        {
            _browser.GoForward();
        }

        public void Refresh()
        {
            _browser.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
