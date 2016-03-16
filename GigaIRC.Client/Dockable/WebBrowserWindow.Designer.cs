namespace GigaIRC.Client.Dockable
{
    partial class WebBrowserWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebBrowserWindow));
            this.Toolbar = new System.Windows.Forms.ToolStrip();
            this.GoBack = new System.Windows.Forms.ToolStripButton();
            this.GoForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Stop = new System.Windows.Forms.ToolStripButton();
            this.RefreshPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Urlbox = new System.Windows.Forms.ToolStripComboBox();
            this.Progress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.Toolbar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Toolbar
            // 
            this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GoBack,
            this.GoForward,
            this.toolStripSeparator1,
            this.Stop,
            this.RefreshPage,
            this.toolStripSeparator2,
            this.Urlbox,
            this.Progress,
            this.toolStripButton1,
            this.toolStripButton2});
            this.Toolbar.Location = new System.Drawing.Point(0, 0);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(803, 25);
            this.Toolbar.TabIndex = 0;
            this.Toolbar.Text = "toolStrip1";
            this.Toolbar.Resize += new System.EventHandler(this.Toolbar_Resize);
            this.Toolbar.Layout += new System.Windows.Forms.LayoutEventHandler(this.Toolbar_Layout);
            this.Toolbar.LayoutCompleted += new System.EventHandler(this.Toolbar_LayoutCompleted);
            // 
            // GoBack
            // 
            this.GoBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.GoBack.Image = ((System.Drawing.Image)(resources.GetObject("GoBack.Image")));
            this.GoBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GoBack.Name = "GoBack";
            this.GoBack.Size = new System.Drawing.Size(23, 22);
            this.GoBack.Text = "Go Back";
            this.GoBack.Click += new System.EventHandler(this.GoBack_Click);
            // 
            // GoForward
            // 
            this.GoForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.GoForward.Image = ((System.Drawing.Image)(resources.GetObject("GoForward.Image")));
            this.GoForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GoForward.Name = "GoForward";
            this.GoForward.Size = new System.Drawing.Size(23, 22);
            this.GoForward.Text = "Go Forward";
            this.GoForward.Click += new System.EventHandler(this.GoForward_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // Stop
            // 
            this.Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Stop.Image = ((System.Drawing.Image)(resources.GetObject("Stop.Image")));
            this.Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(23, 22);
            this.Stop.Text = "Stop";
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // RefreshPage
            // 
            this.RefreshPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshPage.Image = ((System.Drawing.Image)(resources.GetObject("RefreshPage.Image")));
            this.RefreshPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshPage.Name = "RefreshPage";
            this.RefreshPage.Size = new System.Drawing.Size(23, 22);
            this.RefreshPage.Text = "Refresh";
            this.RefreshPage.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // Urlbox
            // 
            this.Urlbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.Urlbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.Urlbox.AutoSize = false;
            this.Urlbox.Name = "Urlbox";
            this.Urlbox.Size = new System.Drawing.Size(121, 23);
            this.Urlbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Urlbox_KeyDown);
            // 
            // Progress
            // 
            this.Progress.AutoSize = false;
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(66, 22);
            this.Progress.Visible = false;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.Browser);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(803, 489);
            this.panel1.TabIndex = 1;
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.Location = new System.Drawing.Point(0, 0);
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(799, 485);
            this.Browser.TabIndex = 2;
            this.Browser.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.Browser_ProgressChanged);
            this.Browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.Browser_Navigating);
            this.Browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Browser_DocumentCompleted);
            this.Browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.Browser_Navigated);
            // 
            // DockingWebBrowser
            // 
            this.ClientSize = new System.Drawing.Size(803, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Toolbar);
            this.Name = "DockingWebBrowser";
            this.Load += new System.EventHandler(this.DockingWebBrowser_Load);
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip Toolbar;
        private System.Windows.Forms.ToolStripButton GoBack;
        private System.Windows.Forms.ToolStripButton GoForward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton Stop;
        private System.Windows.Forms.ToolStripButton RefreshPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox Urlbox;
        private System.Windows.Forms.ToolStripProgressBar Progress;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser Browser;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
