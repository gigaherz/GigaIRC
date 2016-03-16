using GigaIRC.Config;
using GigaIRC.Protocol;

namespace GigaIRC.Client.Preferences
{
    public partial class IdentitiesPage : PreferencesPageBase
    {
        public override string PageName => "Identities";

        public IdentitiesPage()
        {
            InitializeComponent();

        }

        private void IdentitiesPage_Load(object sender, System.EventArgs e)
        {
            Text = "Identity Manager";

            listBox1.DisplayMember = "DescriptiveName";
            foreach (var net in settings.Identities)
            {
                listBox1.Items.Add(net);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
        //public string DescriptiveName { get; set; }
        //public string FullName { get; set; }
        //public string Username { get; set; }
        //public string Nickname { get; set; }
        //public string AltNickname { get; set; }

            var item = listBox1.SelectedItem as Identity;
            if (item == null)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                return;
            }

            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;

            textBox1.Text = item.DescriptiveName;
            textBox2.Text = item.FullName;
            textBox3.Text = item.Username;
            textBox4.Text = item.Nickname;
            textBox4.Text = item.AltNickname;
        }
    }
}
