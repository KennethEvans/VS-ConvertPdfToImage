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
        public string outDir = @"C:\Scratch\AAA\SavePdfImage";
        public string outName;
        public string pdfPath = TEST_PDF;
        public double resolution = 300;

        public MainForm() {
            InitializeComponent();

            if (pdfPath != null) textBoxSrc.Text = pdfPath;
            if (outDir != null) textBoxDest.Text = outDir;
            if (outName == null) {
                textBoxName.Text = DEFAULT_NAME;
            } else {
                textBoxName.Text = outName;
            }
            textBoxRes.Text = resolution.ToString();
        }

        private void onSaveClick(object sender, EventArgs e) {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = outDir;
            process.StartInfo.FileName = gsPath;
            string outPath = textBoxDest.Text + @"\" + textBoxName.Text;
            string inName = textBoxSrc.Text;
            string resolution = textBoxRes.Text;
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
            textBoxInfo.AppendText(NL);

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
        }

        private void onQuitClick(object sender, EventArgs e) {
            Close();
        }

        private void onBrowsePdfClick(object sender, EventArgs e) {
            string initialPath = textBoxSrc.Text;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a PDF File";
            dlg.DefaultExt = ".pdf";
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
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                textBoxDest.Text = dlg.SelectedPath;
            }
        }

        private void onBrowseDestClick(object sender, EventArgs e) {
            string initialPath = textBoxDest.Text;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select the Destination Directory";
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.ValidateNames = false;
            dlg.FileName = "Folder Selection";
            // Set initial directory
            dlg.InitialDirectory = initialPath;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                textBoxDest.Text = dlg.FileName;
            }
        }
    }
}
