using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SavePdfImage {
    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm();

            // Command line
            if (args.Length > 0) {
                string filePath = args[0];
                string dir = Path.GetDirectoryName(filePath);
                FileInfo fileInfo = new FileInfo(filePath);
                string name = fileInfo.Name;
                string ext = fileInfo.Extension;
                string outName = name.Substring(0, name.Length - ext.Length)
                    + ".png";
                mainForm.Outpath = dir + @"\" + outName;
            }
            Application.Run(mainForm);
        }
    }
}
