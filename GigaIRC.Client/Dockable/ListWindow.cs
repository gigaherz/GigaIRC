using System;
using System.Windows.Forms;
using GigaIRC.Protocol;
using WeifenLuo.WinFormsUI.Docking;

namespace GigaIRC.Client.Dockable
{
    public partial class ListWindow : DockContent
    {
        public event IRCOnInputEvent OnInput;

        public Connection Connection { get; set; }
        public string WindowId { get; set; }

        public ListBox Listbox { get; private set; }

        public override string Text
        {
            get { return base.Text; }
            set { 
                base.Text = value;
                TabText = value;
            }
        }

        public bool ShowListbox
        {
            get { return !NickListSplit.Panel2Collapsed; }
            set { NickListSplit.Panel2Collapsed = !value; }
        }

        public bool ShowTitlePanel
        {
            get { return !TopicSplit.Panel1Collapsed; }
            set { TopicSplit.Panel1Collapsed = !value; }
        }

        public string TopicText
        {
            get { return TopicBox.Text; }
            set { TopicBox.Text = value; }
        }

        public string TopicInfo
        {
            get { return TopicInfoLabel.Text; }
            set { TopicInfoLabel.Text = value; }
        }

        public ListWindow(Tuple<Connection, string> id)
        {
            InitializeComponent();

            Connection = id.Item1;
            WindowId = id.Item2;
        }

        private void EditorSplit_SplitterMoved(object sender, SplitterEventArgs e)
        {
            EditBox.ScrollBars = ScrollBars.None;
            EditBox.Multiline = false;

            if (Math.Abs(EditorSplit.Panel2.Height - EditBox.Height) > 5)
            {
                EditBox.Multiline = true;
                EditBox.ScrollBars = ScrollBars.Vertical;
            }
        }

        // IRCList interface implementation
        public void Clear()
        {
            ircTextList1.Clear();
        }

        public void AddLine(int color, string text)
        {
            ircTextList1.AddLine(color, text);
        }

        public void AddLine(int before, int color, string text)
        {
            ircTextList1.AddLine(before, color, text);
        }

        public string GetLine(int number)
        {
            return ircTextList1.GetLine(number);
        }

        public int GetLineColor(int number)
        {
            return ircTextList1.GetLineColor(number);
        }

        public void SetLine(int number, int color, string text)
        {
            ircTextList1.SetLine(number, color, text);
        }

        public void RemoveLine(int number)
        {
            ircTextList1.RemoveLine(number);
        }

        public int LineCount()
        {
            return ircTextList1.LineCount();
        }

        private void EditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            OnInput?.Invoke(this, new OnInputEventArgs(EditBox.Text));
            EditBox.Text = "";
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void ircTextList1_Click(object sender, EventArgs e)
        {
            EditBox.Focus();
        }

        private void DockableIRCWindow_Activated(object sender, EventArgs e)
        {
            EditBox.Focus();
        }

        private void DockableIRCWindow_Load(object sender, EventArgs e)
        {

        }

        private void ircTextList1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void DockableIRCWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }

    public class OnInputEventArgs : EventArgs
    {
        public string Text;

        public OnInputEventArgs(string text)
        {
            Text = text;
        }
    }

    public delegate void IRCOnInputEvent(object sender, OnInputEventArgs e);
}
