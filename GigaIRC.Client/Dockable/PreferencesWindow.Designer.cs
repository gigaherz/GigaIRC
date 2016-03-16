namespace GigaIRC.Client.Dockable
{
    partial class PreferencesWindow
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Display", 5, 5);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Identities", 9, 9);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Networks", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("User Notes", 7, 7);
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Friends and Favorites", 4, 3, new System.Windows.Forms.TreeNode[] {
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("DCC (Downloads)", 1, 1);
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Security & Privacy", 6, 6);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesWindow));
            this.PageSelector = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.PageContainer = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // PageSelector
            // 
            this.PageSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PageSelector.ImageIndex = 10;
            this.PageSelector.ImageList = this.imageList1;
            this.PageSelector.Location = new System.Drawing.Point(12, 12);
            this.PageSelector.Name = "PageSelector";
            treeNode8.ImageIndex = 5;
            treeNode8.Name = "Display";
            treeNode8.SelectedImageIndex = 5;
            treeNode8.Tag = "Display";
            treeNode8.Text = "Display";
            treeNode9.ImageIndex = 9;
            treeNode9.Name = "Identities";
            treeNode9.SelectedImageIndex = 9;
            treeNode9.Tag = "Identities";
            treeNode9.Text = "Identities";
            treeNode10.ImageIndex = 0;
            treeNode10.Name = "Networks";
            treeNode10.SelectedImageIndex = 0;
            treeNode10.Tag = "Networks";
            treeNode10.Text = "Networks";
            treeNode11.ImageIndex = 7;
            treeNode11.Name = "Notes";
            treeNode11.SelectedImageIndex = 7;
            treeNode11.Tag = "UserNotes";
            treeNode11.Text = "User Notes";
            treeNode12.ImageIndex = 4;
            treeNode12.Name = "Favorites";
            treeNode12.SelectedImageIndex = 3;
            treeNode12.Tag = "Friends";
            treeNode12.Text = "Friends and Favorites";
            treeNode13.ImageIndex = 1;
            treeNode13.Name = "DCC";
            treeNode13.SelectedImageIndex = 1;
            treeNode13.Tag = "DCC";
            treeNode13.Text = "DCC (Downloads)";
            treeNode14.ImageIndex = 6;
            treeNode14.Name = "Security";
            treeNode14.SelectedImageIndex = 6;
            treeNode14.Tag = "Security";
            treeNode14.Text = "Security & Privacy";
            this.PageSelector.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode10,
            treeNode12,
            treeNode13,
            treeNode14});
            this.PageSelector.SelectedImageIndex = 10;
            this.PageSelector.ShowLines = false;
            this.PageSelector.ShowRootLines = false;
            this.PageSelector.Size = new System.Drawing.Size(169, 554);
            this.PageSelector.TabIndex = 0;
            this.PageSelector.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Globe16.png");
            this.imageList1.Images.SetKeyName(1, "Downloads16.png");
            this.imageList1.Images.SetKeyName(2, "Balloon16.png");
            this.imageList1.Images.SetKeyName(3, "Address-book-open16.png");
            this.imageList1.Images.SetKeyName(4, "Address-book16.png");
            this.imageList1.Images.SetKeyName(5, "Monitor16.png");
            this.imageList1.Images.SetKeyName(6, "Key16.png");
            this.imageList1.Images.SetKeyName(7, "Note16.png");
            this.imageList1.Images.SetKeyName(8, "Star-gold16.png");
            this.imageList1.Images.SetKeyName(9, "Person16.png");
            this.imageList1.Images.SetKeyName(10, "Advanced16.png");
            // 
            // PageContainer
            // 
            this.PageContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PageContainer.Location = new System.Drawing.Point(187, 12);
            this.PageContainer.Name = "PageContainer";
            this.PageContainer.Padding = new System.Windows.Forms.Padding(8);
            this.PageContainer.Size = new System.Drawing.Size(566, 554);
            this.PageContainer.TabIndex = 1;
            this.PageContainer.TabStop = false;
            this.PageContainer.Text = "<Current Page Name>";
            // 
            // PreferencesWindow
            // 
            this.ClientSize = new System.Drawing.Size(765, 578);
            this.Controls.Add(this.PageContainer);
            this.Controls.Add(this.PageSelector);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PreferencesWindow";
            this.Text = "Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreferencesWindow_FormClosed);
            this.Load += new System.EventHandler(this.DockingPreferences_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView PageSelector;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox PageContainer;
    }
}
