using GigaIRC.Config;

namespace GigaIRC.Client.WPF.Preferences
{
    public partial class Identities
    {
        public Settings Settings => MainWindow.Instance.Session.Settings;

        public Identities()
        {
            InitializeComponent();
        }
    }
}
