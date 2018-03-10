using System.Diagnostics;
using System.IO;

namespace TfsMigrate.Core.Exporter
{
    public class GitClient : IGitClient
    {
        private readonly string _path;

        public GitClient(string repositoryPath)
        {
            _path = repositoryPath;
            Directory.CreateDirectory(_path);
        }

        public void Init()
        {
            RunGitCommand("init").WaitForExit();
        }

        public GitStreamWriter CreateGitStreamWriter()
        {
            return new GitStreamWriter(FastImport());
        }

        public void AddUpstreamRemote(string url)
        {
            RunGitCommand($"remote add upstream {url}").WaitForExit();
        }

        public void PushAllUpstreamRemote()
        {
            RunGitCommand("push upstream --mirror").WaitForExit();
        }

        private Process FastImport()
        {
            return RunGitCommand("fast-import");
        }

        private Process RunGitCommand(string arguments)
        {
            var processStartInfo = new ProcessStartInfo("git")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = _path,
                CreateNoWindow = true,
                Arguments = arguments
            };

            var process = Process.Start(processStartInfo);
            return process;
        }
    }
}
