using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SavePdfImage {

    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        private static readonly string DEFAULT_NAME = "SavePdfImage.png";
        private static readonly string TEST_PDF = @"C:\Users\evans\Documents\Family Historian Projects\Evans\Public\Evans-Descendant-2019-11-21.PDFCreator.pdf";
        public string gsPath = @"C:\Program Files\gs\gs9.20\bin\gswin64c";
        private string outpath = @"C:\Scratch\AAA\SavePdfImage" + @"\" + DEFAULT_NAME;
        public string pdfPath = TEST_PDF;
        public double resolution = 300;

        public string Outpath
        {
            get => outpath;
            set
            {
                outpath = value;
                textBoxDest.Text = value;
            }
        }

        public MainForm() {
            InitializeComponent();

            if (pdfPath != null) textBoxSrc.Text = pdfPath;
            if (Outpath != null) textBoxDest.Text = Outpath;
            textBoxRes.Text = resolution.ToString();
        }

        private void onSaveClick(object sender, EventArgs e) {
            string inName = textBoxSrc.Text;
            textBoxInfo.AppendText("Processing " + inName + NL);
            if (!File.Exists(inName)) {
                textBoxInfo.AppendText(NL + "Does not exist: " + inName
                    + NL + NL);
                return;
            }
            string outPath = textBoxDest.Text;
            string outPathDir = Path.GetDirectoryName(outPath);
            if (!Directory.Exists(outPathDir)) {
                textBoxInfo.AppendText(NL + "Directory does not exist: "
                    + outPathDir + NL + NL);
                return;
            }
            string resolution = textBoxRes.Text;
            try {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = outPathDir;
                process.StartInfo.FileName = gsPath;
                process.StartInfo.Arguments =
                    "-dNOPAUSE -dBATCH -r" + resolution
                    + " -sDEVICE=png16m"
                    + " -sOutputFile=\"" + outPath + "\" \"" + inName + "\"";

                textBoxInfo.AppendText(process.StartInfo.FileName + " "
                    + process.StartInfo.Arguments + NL);

                process.Start();
                process.WaitForExit();

                // Stdout
                string stdout = "";
                using (System.IO.StreamReader myOutput = process.StandardOutput) {
                    stdout = myOutput.ReadToEnd();
                }
                textBoxInfo.AppendText(NL);
                textBoxInfo.AppendText("STDOUT:" + NL);
                textBoxInfo.AppendText(stdout.Replace("\n", NL));

                // Stderr
                string stderr = "";
                using (System.IO.StreamReader myError = process.StandardError) {
                    stderr = myError.ReadToEnd();
                }
                textBoxInfo.AppendText(NL);
                textBoxInfo.AppendText("STDERR:" + NL);
                textBoxInfo.AppendText(stderr.Replace("\n", NL));

                // Check exit code
                if (process.ExitCode == 0) {
                    textBoxInfo.AppendText(NL);
                    textBoxInfo.AppendText("Wrote " + outPath + NL);
                    textBoxInfo.AppendText(NL);
                    textBoxInfo.AppendText("All Done");
                } else {
                    textBoxInfo.AppendText(NL);
                    textBoxInfo.AppendText("Aborted");
                }
            } catch (Exception ex) {
                textBoxInfo.AppendText(NL);
                textBoxInfo.AppendText("Falied to extract image" + NL);
                textBoxInfo.AppendText(ex.Message + NL);
            }
            textBoxInfo.AppendText(NL);
        }

        private void onQuitClick(object sender, EventArgs e) {
            Close();
        }

        private void onBrowsePdfClick(object sender, EventArgs e) {
            string initialPath = textBoxSrc.Text;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a PDF File";
            dlg.Filter = "PDF File|*.pdf";
            // Set initial directory
            if (File.Exists(initialPath)) {
                dlg.FileName = Path.GetFileName(initialPath);
                dlg.InitialDirectory = Path.GetDirectoryName(initialPath);
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                textBoxSrc.Text = dlg.FileName;
            }
        }

        private void onBrowseDestClick0(object sender, EventArgs e) {
            string outPath = textBoxDest.Text;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select the Destination Folder";
            // Set initial directory
            if (File.Exists(outPath)) {
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                dlg.SelectedPath = outPath;
            }
            if (dlg.ShowDialog() == DialogResult.OK) {
                textBoxDest.Text = dlg.SelectedPath;
            }
        }

        private void onBrowseDestClick(object sender, EventArgs e) {
            string initialPath = textBoxDest.Text;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Select an Output File";
            dlg.Filter = "PNG Image|*.png";
            // Set initial directory
            if (File.Exists(initialPath)) {
                dlg.FileName = Path.GetFileName(initialPath);
                dlg.InitialDirectory = Path.GetDirectoryName(initialPath);
            }
            if (dlg.ShowDialog() == DialogResult.OK) {
                textBoxDest.Text = dlg.FileName;
            }
        }

        private void onSettingsClick(object sender, EventArgs e) {
            InputDialog dlg = new InputDialog(this);
            dlg.GsPath = gsPath;
            if (dlg.ShowDialog() == DialogResult.OK) {
                gsPath = dlg.GsPath;
                textBoxInfo.AppendText("Resetting GhostScript executable to "
                    + gsPath + NL);
            }
            dlg.Dispose();
        }
    }
}
