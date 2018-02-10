using System.Diagnostics;
using System.IO;

namespace TfsMigrate.Core.Exporter
{
    public class GitStreamWriter : StreamWriter
    {
        public GitStreamWriter(string path) : base(CreateGitFastImportStream(path))
        {
            NewLine = "\n";
            AutoFlush = true;
        }

        private static Stream CreateGitFastImportStream(string outputPath)
        {
            return File.Create(@"C:\Users\alastair.gould\Documents\output2.export");

            System.IO.Directory.CreateDirectory(outputPath);

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = outputPath;

            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine("git init");
                process.StandardInput.WriteLine("git fast-import");
            }

            return process.StandardInput.BaseStream;
        }
    }
}
