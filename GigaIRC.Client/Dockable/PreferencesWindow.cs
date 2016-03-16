using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GigaIRC.Client.Preferences;

namespace GigaIRC.Client.Dockable
{
    public partial class PreferencesWindow : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private readonly Dictionary<string, PreferencesPageBase> controlCache = new Dictionary<string, PreferencesPageBase>();

        private PreferencesPageBase curPage;

        public PreferencesWindow()
        {
            InitializeComponent();
        }

        private void LoadPage(string pageName)
        {
            PreferencesPageBase newPage;

            if (!controlCache.TryGetValue(pageName, out newPage))
            {
                for (var type = Type.GetType($"GigaIRC.Client.Preferences.{pageName}Page", false, false);
                    type != null; type = null)
                {
                    newPage = (PreferencesPageBase) Activator.CreateInstance(type);
                }

                controlCache.Add(pageName, newPage);
            }

            SetContainer(newPage);
        }

        private void SetContainer(PreferencesPageBase newPage)
        {
            if (curPage != null)
            {
                if (newPage == curPage)
                    return;

                curPage.Visible = false;
                curPage = null;
            }

            PageContainer.Text = newPage != null ? newPage.Text : "Unimplemented page";

            curPage = newPage;

            if (curPage == null)
                return;

            curPage.Parent = PageContainer;
            curPage.Dock = DockStyle.Fill;
            curPage.Visible = true;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = (string)e.Node.Tag;

            LoadPage(node);
        }

        private void DockingPreferences_Load(object sender, EventArgs e)
        {
            PageSelector.ExpandAll();
        }

        private void PreferencesWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            curPage = null;

            foreach (var page in controlCache.Values)
                page?.Dispose();
        }
    }
}
