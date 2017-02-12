using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace ConvertPdfToImage {
    class Program {
        const string GS = @"C:\Program Files\gs\gs9.20\bin\gswin64c";
        const string OUTDIR = @"C:\Scratch\ECG\AliveCor ECGs\Images";

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("No input file given");
                System.Environment.Exit(1);
            }
            string filePath = args[0];
            Console.WriteLine("Processing " + filePath);
            FileInfo fileInfo = new FileInfo(filePath);
            string name = fileInfo.Name;
            string ext = fileInfo.Extension;
            string outName = name.Substring(0, name.Length - ext.Length)
                + ".%02d.png";
            string outPath = OUTDIR + @"\" + outName;
            //Console.WriteLine(name);
            //Console.WriteLine(ext);
            //Console.WriteLine(outName);
            //Console.WriteLine(outPath);

            // Run the command
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = OUTDIR;
            process.StartInfo.FileName = GS;
            process.StartInfo.Arguments =
                "-dNOPAUSE -dBATCH -r300 -sDEVICE=png16m -sOutputFile=\""
                + outPath + "\" \"" + filePath + "\"";

            Console.WriteLine();
            Console.WriteLine(process.StartInfo.FileName + " "
                + process.StartInfo.Arguments);

            process.Start();
            process.WaitForExit();

            // Stdout
            string stdout = "";
            using (System.IO.StreamReader myOutput = process.StandardOutput) {
                stdout = myOutput.ReadToEnd();
            }
            Console.WriteLine();
            Console.WriteLine("STDOUT:");
            Console.WriteLine(stdout);

            // Stderr
            string stderr = "";
            using (System.IO.StreamReader myError = process.StandardError) {
                stderr = myError.ReadToEnd();
            }
            Console.WriteLine();
            Console.WriteLine("STDERR:");
            Console.WriteLine(stderr);

            // Check exit code
            if (process.ExitCode == 0) {
                Console.WriteLine();
                Console.WriteLine("Wrote " + outName);
                Console.WriteLine();
                Console.WriteLine("All Done");
            } else {
                Console.WriteLine();
                Console.WriteLine("Aborted");
            }

            // Prompt to exit so the console will stay up
            Console.WriteLine("Enter any key to exit:");
            string line = Console.ReadLine();
        }
    }
}
