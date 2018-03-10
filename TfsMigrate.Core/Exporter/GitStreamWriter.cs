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

        public string ReadResponse()
        {
            return _process.StandardOutput.ReadLine();
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

    }
}
