using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
