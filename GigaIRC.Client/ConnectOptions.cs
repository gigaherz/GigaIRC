using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GigaIRC
{
    public partial class ConnectOptions : Form
    {
        public string FullName { get { return txtFullName.Text; } set { txtFullName.Text = value; } }
        public string Username { get { return txtUsername.Text; } set { txtUsername.Text = value; } }
        public string Nickname { get { return txtNickname.Text; } set { txtNickname.Text = value; } }
        public string AltNickname { get { return txtAltNickname.Text; } set { txtAltNickname.Text = value; } }
        public string Server { get { return txtServer.Text; } set { txtServer.Text = value; } }
        public string Port { get { return txtPort.Text; } set { txtPort.Text = value; } }

        public ConnectOptions()
        {
            InitializeComponent();
        }

        private void Port_Validating(object sender, CancelEventArgs e)
        {
            ushort dummy;
            e.Cancel = txtPort.TextLength > 0 && !ushort.TryParse(txtPort.Text, out dummy);
        }

        private void ConnectOptions_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
