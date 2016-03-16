using GigaIRC.Client.RichList;
using GigaIRC.RichList;

namespace GigaIRC.Client.Dockable
{
    partial class ListWindow
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
            this.TopicSplit = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.TopicInfoLabel = new System.Windows.Forms.Label();
            this.TopicBox = new System.Windows.Forms.TextBox();
            this.EditorSplit = new System.Windows.Forms.SplitContainer();
            this.NickListSplit = new System.Windows.Forms.SplitContainer();
            this.ircTextList1 = new WpfRichList();
            this.Listbox = new System.Windows.Forms.ListBox();
            this.EditBox = new System.Windows.Forms.TextBox();
            this.TopicSplit.Panel1.SuspendLayout();
            this.TopicSplit.Panel2.SuspendLayout();
            this.TopicSplit.SuspendLayout();
            this.EditorSplit.Panel1.SuspendLayout();
            this.EditorSplit.Panel2.SuspendLayout();
            this.EditorSplit.SuspendLayout();
            this.NickListSplit.Panel1.SuspendLayout();
            this.NickListSplit.Panel2.SuspendLayout();
            this.NickListSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopicSplit
            // 
            this.TopicSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopicSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.TopicSplit.IsSplitterFixed = true;
            this.TopicSplit.Location = new System.Drawing.Point(0, 0);
            this.TopicSplit.Name = "TopicSplit";
            this.TopicSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // TopicSplit.Panel1
            // 
            this.TopicSplit.Panel1.Controls.Add(this.button1);
            this.TopicSplit.Panel1.Controls.Add(this.TopicInfoLabel);
            this.TopicSplit.Panel1.Controls.Add(this.TopicBox);
            this.TopicSplit.Panel1MinSize = 20;
            // 
            // TopicSplit.Panel2
            // 
            this.TopicSplit.Panel2.Controls.Add(this.EditorSplit);
            this.TopicSplit.Size = new System.Drawing.Size(554, 387);
            this.TopicSplit.SplitterDistance = 38;
            this.TopicSplit.TabIndex = 2;
            this.TopicSplit.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(522, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "set";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // TopicInfoLabel
            // 
            this.TopicInfoLabel.AutoSize = true;
            this.TopicInfoLabel.Location = new System.Drawing.Point(3, 23);
            this.TopicInfoLabel.Name = "TopicInfoLabel";
            this.TopicInfoLabel.Size = new System.Drawing.Size(67, 13);
            this.TopicInfoLabel.TabIndex = 1;
            this.TopicInfoLabel.Text = "No Topic set.";
            // 
            // TopicBox
            // 
            this.TopicBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TopicBox.BackColor = System.Drawing.SystemColors.Window;
            this.TopicBox.Location = new System.Drawing.Point(0, 0);
            this.TopicBox.Name = "TopicBox";
            this.TopicBox.Size = new System.Drawing.Size(522, 20);
            this.TopicBox.TabIndex = 0;
            this.TopicBox.TabStop = false;
            // 
            // EditorSplit
            // 
            this.EditorSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.EditorSplit.Location = new System.Drawing.Point(0, 0);
            this.EditorSplit.Name = "EditorSplit";
            this.EditorSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // EditorSplit.Panel1
            // 
            this.EditorSplit.Panel1.Controls.Add(this.NickListSplit);
            // 
            // EditorSplit.Panel2
            // 
            this.EditorSplit.Panel2.Controls.Add(this.EditBox);
            this.EditorSplit.Panel2MinSize = 20;
            this.EditorSplit.Size = new System.Drawing.Size(554, 345);
            this.EditorSplit.SplitterDistance = 321;
            this.EditorSplit.TabIndex = 0;
            this.EditorSplit.TabStop = false;
            this.EditorSplit.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.EditorSplit_SplitterMoved);
            // 
            // NickListSplit
            // 
            this.NickListSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NickListSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.NickListSplit.Location = new System.Drawing.Point(0, 0);
            this.NickListSplit.Name = "NickListSplit";
            // 
            // NickListSplit.Panel1
            // 
            this.NickListSplit.Panel1.Controls.Add(this.ircTextList1);
            // 
            // NickListSplit.Panel2
            // 
            this.NickListSplit.Panel2.Controls.Add(this.Listbox);
            this.NickListSplit.Panel2MinSize = 30;
            this.NickListSplit.Size = new System.Drawing.Size(554, 321);
            this.NickListSplit.SplitterDistance = 463;
            this.NickListSplit.TabIndex = 0;
            this.NickListSplit.TabStop = false;
            // 
            // ircTextList1
            // 
            this.ircTextList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ircTextList1.Location = new System.Drawing.Point(0, 0);
            this.ircTextList1.Name = "ircTextList1";
            this.ircTextList1.Size = new System.Drawing.Size(463, 321);
            this.ircTextList1.TabIndex = 0;
            this.ircTextList1.Click += new System.EventHandler(this.ircTextList1_Click);
            this.ircTextList1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ircTextList1_PreviewKeyDown);
            // 
            // NickList
            // 
            this.Listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Listbox.FormattingEnabled = true;
            this.Listbox.IntegralHeight = false;
            this.Listbox.Location = new System.Drawing.Point(0, 0);
            this.Listbox.Name = "Listbox";
            this.Listbox.Size = new System.Drawing.Size(87, 321);
            this.Listbox.TabIndex = 1;
            // 
            // EditBox
            // 
            this.EditBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditBox.Location = new System.Drawing.Point(0, 0);
            this.EditBox.Name = "EditBox";
            this.EditBox.Size = new System.Drawing.Size(554, 20);
            this.EditBox.TabIndex = 0;
            this.EditBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditBox_KeyDown);
            // 
            // DockableIRCWindow
            // 
            this.ClientSize = new System.Drawing.Size(554, 387);
            this.Controls.Add(this.TopicSplit);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DockableIRCWindow";
            this.TabText = "DockableForm";
            this.Text = "DockableForm";
            this.Activated += new System.EventHandler(this.DockableIRCWindow_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DockableIRCWindow_FormClosing);
            this.Load += new System.EventHandler(this.DockableIRCWindow_Load);
            this.TopicSplit.Panel1.ResumeLayout(false);
            this.TopicSplit.Panel1.PerformLayout();
            this.TopicSplit.Panel2.ResumeLayout(false);
            this.TopicSplit.ResumeLayout(false);
            this.EditorSplit.Panel1.ResumeLayout(false);
            this.EditorSplit.Panel2.ResumeLayout(false);
            this.EditorSplit.Panel2.PerformLayout();
            this.EditorSplit.ResumeLayout(false);
            this.NickListSplit.Panel1.ResumeLayout(false);
            this.NickListSplit.Panel2.ResumeLayout(false);
            this.NickListSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer TopicSplit;
        private System.Windows.Forms.SplitContainer EditorSplit;
        private System.Windows.Forms.SplitContainer NickListSplit;
        private System.Windows.Forms.TextBox EditBox;
        private System.Windows.Forms.Label TopicInfoLabel;
        private System.Windows.Forms.TextBox TopicBox;
        private System.Windows.Forms.Button button1;
        private WpfRichList ircTextList1;

    }
}
