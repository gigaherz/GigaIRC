using System.Windows.Forms;
using GigaIRC.Config;
using GigaIRC.Protocol;

namespace GigaIRC.Client.Preferences
{
    public partial class PreferencesPageBase : UserControl
    {
        protected readonly Settings settings;

        public virtual string PageName => "<generic page>";

        public PreferencesPageBase()
        {
            InitializeComponent();

            if(MainForm.Instance != null)
                settings = MainForm.Instance.Session.Settings;
        }

    }
}
