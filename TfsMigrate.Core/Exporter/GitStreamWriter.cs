using System.Diagnostics;
using System.IO;

namespace TfsMigrate.Core.Exporter
{
    public class GitStreamWriter : StreamWriter
    {
        private readonly Process _process;

        private bool _disposed;

        public GitStreamWriter(Process process) : base(process.StandardInput.BaseStream)
        {
            _process = process;
            NewLine = "\n";
            AutoFlush = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _process.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        public static GitStreamWriter CreateGitStreamWriter(string repositoryPath)
        {
            return new GitStreamWriter(CreateGitFastImportProcess(repositoryPath));
        }

        private static Process CreateGitFastImportProcess(string repositoryPath)
        {
            Directory.CreateDirectory(repositoryPath);

            var processStartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = repositoryPath
            };

            var process = Process.Start(processStartInfo);

            if (process != null)
            {
                process.StandardInput.WriteLine("git init");
                process.StandardInput.WriteLine("git fast-import");
            }

            return process;
        }
    }
}
