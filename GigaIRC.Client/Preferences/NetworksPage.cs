using System;
using GigaIRC.Config;
using GigaIRC.Protocol;

namespace GigaIRC.Client.Preferences
{
    public partial class NetworksPage : PreferencesPageBase
    {
        public override string PageName => "Networks";

        public NetworksPage()
        {
            InitializeComponent();
        }

        private void NetworksPage_Load(object sender, EventArgs e)
        {
            Text = "Network Manager";

            listBox1.DisplayMember = "Name";
            foreach(var net in settings.Networks)
            {
                listBox1.Items.Add(net);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = -1;
            listBox2.Items.Clear();

            var item = listBox1.SelectedItem as Network;
            if (item == null)
            {
                textBox1.Text = "";
                groupBox1.Enabled = false;
                return;
            }

            groupBox1.Enabled = true;
            textBox1.Text = item.Name;
            listBox2.DisplayMember = "DisplayName";
            foreach(var svr in item.Servers)
            {
                listBox2.Items.Add(svr);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBox2.SelectedItem as Server;
            if (item == null)
            {
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                return;
            }

            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;

            textBox2.Text = item.DisplayName;
            textBox3.Text = item.Address;
            textBox4.Text = item.GetPortRanges();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void textBox3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var item = listBox2.SelectedItem as Server;
            if (item == null)
            {
                return;
            }

            if(!item.SetPortRanges(textBox4.Text))
            {
                e.Cancel = true;
            }
        }
    }
}
