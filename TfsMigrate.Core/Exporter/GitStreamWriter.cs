using System.Diagnostics;
using System.IO;

namespace TfsMigrate.Core.Exporter
{
    public class GitStreamWriter : StreamWriter
    {
        private readonly Process process;

        private bool disposed = false;

        public GitStreamWriter(Process process) : base(process.StandardInput.BaseStream)
        {
            this.process = process;
            NewLine = "\n";
            AutoFlush = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                process.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }

        public static GitStreamWriter CreateGitStreamWriter(string repositoryPath)
        {
            return new GitStreamWriter(CreateGitFastImportProcess(repositoryPath));
        }

        private static Process CreateGitFastImportProcess(string repositoryPath)
        {
            Directory.CreateDirectory(repositoryPath);

            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = repositoryPath;

            Process process = Process.Start(processStartInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine("git init");
                process.StandardInput.WriteLine("git fast-import");
            }

            return process;
        }
    }
}
