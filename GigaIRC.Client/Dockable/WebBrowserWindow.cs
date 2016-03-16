using System;
using System.Windows.Forms;

namespace GigaIRC.Client.Dockable
{
    public partial class WebBrowserWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public WebBrowserWindow()
        {
            InitializeComponent();
            Browser.CanGoBackChanged += Browser_CanGoBackChanged;
            Browser.CanGoForwardChanged += Browser_CanGoForwardChanged;
        }

        private void DockingWebBrowser_Load(object sender, EventArgs e)
        {
            Browser.Navigate("about:blank");
        }

        private void Browser_CanGoBackChanged(object sender, EventArgs e)
        {
            GoBack.Enabled = false;
        }

        private void Browser_CanGoForwardChanged(object sender, EventArgs e)
        {
            GoForward.Enabled = false;
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Urlbox.Text = e.Url.ToString();
            Progress.Visible = true;
            Progress.Value = 0;
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Urlbox.Text = e.Url.ToString();
        }

        private void Browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.MaximumProgress > 0)
                Progress.Value = (int)(e.CurrentProgress * 100 / e.MaximumProgress);
            else
                Progress.Value = 0;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            Browser.Refresh();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            Browser.Stop();
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Browser.GoBack();
        }

        private void GoForward_Click(object sender, EventArgs e)
        {
            Browser.GoForward();
        }

        private void Urlbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Browser.Navigate(Urlbox.Text);
                e.Handled = true;
            }
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Progress.Visible = false;
            Urlbox.Text = e.Url.ToString();
            if(Browser.Document != null && Browser.Document.Title.Length>0)
                Text = Browser.Document.Title;
        }

        private void Toolbar_Resize(object sender, EventArgs e)
        {
            /*
            int widthleft = Width - Toolbar.GripMargin.Size.Width;
            int controlsprocessed = 0;
            foreach (ToolStripItem c in Toolbar.Items)
            {
                if ((c != Urlbox) && (c.Visible))
                {
                    widthleft -= (c.Width + c.Padding.Size.Width + Toolbar.Margin.Size.Width + 8);
                    controlsprocessed++;
                }
            }
            if (widthleft > 20)
            {
                Urlbox.Width = widthleft - (Urlbox.Padding.Size.Width + Toolbar.Margin.Size.Width);
            }
             * */
        }

        private void Toolbar_Layout(object sender, LayoutEventArgs e)
        {
            Urlbox.Width = 20;
        }

        bool stillLayingOut;
        private void Toolbar_LayoutCompleted(object sender, EventArgs e)
        {
            if (stillLayingOut) return;

            stillLayingOut = true;
            int widthleft = Toolbar.ClientSize.Width - Toolbar.GripMargin.Size.Width;
            foreach (ToolStripItem c in Toolbar.Items)
            {
                if ((c != Urlbox) && (c.Visible))
                {
                    widthleft -= (c.Width + c.Padding.Size.Width + Toolbar.Margin.Size.Width + 8);
                }
            }
            if (widthleft > 20)
            {
                Urlbox.Width = widthleft - (Urlbox.Padding.Size.Width + Toolbar.Margin.Size.Width);
            }
            stillLayingOut = false;
            //Toolbar.ResumeLayout();
        }
    }
}
