using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SavePdfImage {

    public partial class MainForm : Form {
        public static readonly String NL = Environment.NewLine;
        private static readonly string DEFAULT_NAME = "SavePdfImage.png";
        private static readonly string TEST_PDF = "";
        //private static readonly string TEST_PDF = @"C:\Users\evans\Documents\Family Historian Projects\Evans\Public\Evans-Descendant-2019-11-21.PDFCreator.pdf";
        public string gsPath = @"C:\Program Files\gs\gs9.20\bin\gswin64c";
        private string outpath =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            + @"\" + DEFAULT_NAME;
        private string pdfPath = "";
        public double resolution = 300;

        public string OutPath
        {
            get => outpath;
            set
            {
                outpath = value;
                textBoxOut.Text = value;
                textBoxOut.SelectionStart = textBoxOut.Text.Length;
                textBoxOut.SelectionLength = 0;
            }
        }

        public string PdfPath
        {
            get => pdfPath;
            set
            {
                pdfPath = value;
                textBoxPdf.Text = value;
                textBoxPdf.SelectionStart = textBoxPdf.Text.Length;
                textBoxPdf.SelectionLength = 0;
                string outDir = Path.GetDirectoryName(OutPath);
                string outName = Path.GetFileName(pdfPath);
                string outExt = Path.GetExtension(pdfPath);
                OutPath = outDir + @"\" + outName.Substring(0,
                    outName.Length - outExt.Length) + ".png";
            }
        }

        public MainForm() {
            InitializeComponent();

            string savedOutPath = Properties.Settings.Default.OutputDir;
            if (savedOutPath != null && savedOutPath.Length > 0) {
                OutPath = savedOutPath + @"\" + DEFAULT_NAME;
            }
            double savedResolution = Properties.Settings.Default.Resolution;
            if (savedResolution > 0) {
                resolution = savedResolution;
            }

            if (OutPath != null) textBoxOut.Text = OutPath;
            // Do this after OutPath
            if (PdfPath != null) textBoxPdf.Text = PdfPath;
            textBoxRes.Text = resolution.ToString();
        }

        private void onSaveClick(object sender, EventArgs e) {
            string pdfPath = textBoxPdf.Text;
            textBoxInfo.AppendText("Processing " + pdfPath + NL);
            if (!File.Exists(pdfPath)) {
                textBoxInfo.AppendText(NL + "Does not exist: " + pdfPath
                    + NL);
                textBoxInfo.AppendText(NL + "Aborted" + NL + NL);
                return;
            }
            string inExt = Path.GetExtension(pdfPath);
            if (!inExt.ToLower().Equals(".pdf")) {
                textBoxInfo.AppendText(NL + "PDF file extension must be .pdf: "
                    + inExt + NL);
                textBoxInfo.AppendText(NL + "Aborted" + NL + NL);
                return;
            }
            string outPath = textBoxOut.Text;
            string outPathDir = Path.GetDirectoryName(outPath);
            string outPathExt = Path.GetExtension(outPath);
            if (!Directory.Exists(outPathDir)) {
                textBoxInfo.AppendText(NL + "Directory does not exist: "
                    + outPathDir + NL);
                textBoxInfo.AppendText(NL + "Aborted" + NL + NL);
                return;
            }
            if (!outPathExt.ToLower().Equals(".png")) {
                textBoxInfo.AppendText(NL + "Output file extension must be .png: "
                    + outPathDir + NL);
                textBoxInfo.AppendText(NL + "Aborted" + NL + NL);
                return;
            }
            // Check if files with this prefix exist
            string outPathPrefix = outPath.Substring(0,
                outPath.Length - outPathExt.Length);
            string[] existingFiles = Directory.GetFiles(outPathDir);
            bool prompt = false;
            int nFiles = 0;
            foreach (string file in existingFiles) {
                if (file.StartsWith(outPathPrefix + ".") &&
                    !outPathPrefix.ToLower().EndsWith(".png")) {
                    nFiles++;
                    prompt = true;
                    break;
                }
            }
            if (prompt) {
                DialogResult res = MessageBox.Show(nFiles
                    + " file[s] of this pattern:" + NL
                    + outPathPrefix + ".*.png" + NL
                    + "already exist." + NL + "Do you want to continue?",
              "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
              MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No) {
                    textBoxInfo.AppendText(NL + "Aborted" + NL + NL);
                    return;
                }
            }
            string resolution = textBoxRes.Text;
            try {
                // Add format for multiple images
                outPath = outPath.Substring(0, outPath.Length - outPathExt.Length)
                + ".%02d.png";
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
                    + " -sOutputFile=\"" + outPath + "\" \"" + pdfPath + "\"";

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
                    textBoxInfo.AppendText(NL + "All Done" + NL);
                } else {
                    textBoxInfo.AppendText(NL + "Aborted" + NL);
                }
            } catch (Exception ex) {
                textBoxInfo.AppendText(NL);
                textBoxInfo.AppendText("Failed to extract image" + NL);
                textBoxInfo.AppendText(ex.Message + NL);
            }
            textBoxInfo.AppendText(NL);
        }

        private void onQuitClick(object sender, EventArgs e) {
            Close();
        }

        private void onBrowsePdfClick(object sender, EventArgs e) {
            string initialPath = textBoxPdf.Text;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Select a PDF File";
            dlg.Filter = "PDF File|*.pdf";
            // Set initial directory
            if (File.Exists(initialPath)) {
                dlg.FileName = Path.GetFileName(initialPath);
                dlg.InitialDirectory = Path.GetDirectoryName(initialPath);
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                PdfPath = dlg.FileName;
            }
        }

        private void onBrowseDestClick(object sender, EventArgs e) {
            string initialPath = textBoxOut.Text;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Select an Output File";
            dlg.Filter = "PNG Image|*.png";
            // Set initial directory
            if (File.Exists(initialPath)) {
                dlg.FileName = Path.GetFileName(initialPath);
                dlg.InitialDirectory = Path.GetDirectoryName(initialPath);
            }
            if (dlg.ShowDialog() == DialogResult.OK) {
                OutPath = dlg.FileName;
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (gsPath != null && gsPath.Length > 0) {
                Properties.Settings.Default.GhostScriptExe = gsPath;
            }
            string text = textBoxOut.Text;
            if (text != null && text.Length > 0) {
                Properties.Settings.Default.OutputDir =
                    Path.GetDirectoryName(text);
            }
            text = textBoxRes.Text;
            if (text != null && text.Length > 0) {
                try {
                    Properties.Settings.Default.Resolution = Double.Parse(text);
                } catch (Exception ex) {
                    // Do nothing
                }
            }
            Properties.Settings.Default.Save();
        }
    }
}
