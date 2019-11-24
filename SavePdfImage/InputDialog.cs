using System;
using System.Windows.Forms;

namespace SavePdfImage {
    public partial class InputDialog : Form {
        private string gsPath = @"C:\Program Files\gs\gs9.20\bin\gswin64c";
        private MainForm mainForm;

        public string GsPath { get => gsPath; set => gsPath = value; }

        public InputDialog() {
            InitializeComponent();
            textBoxGs.Text = GsPath;
        }

        public InputDialog(MainForm mainForm) {
            this.mainForm = mainForm;
            GsPath = mainForm.gsPath;
            InitializeComponent();
            textBoxGs.Text = GsPath;
        }

        private void onCancelClick(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void onOkClick(object sender, EventArgs e) {
            gsPath = textBoxGs.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void onBrowseGhostScriptClick(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a GhostScript executable";
            dlg.Filter = "EXE File|*.exe";
            // Set initial directory to Program Files
            dlg.InitialDirectory =
                 Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                GsPath = dlg.FileName;
                textBoxGs.Text = GsPath;
                mainForm.gsPath = GsPath;
                Properties.Settings.Default.GhostScriptExe = GsPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
