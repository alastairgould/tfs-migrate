using System.Diagnostics;

namespace TfsMigrate.Core.Exporter
{
    public interface IGitClient
    {
        void Init();
        GitStreamWriter CreateGitStreamWriter();
        void AddUpstreamRemote(string url);
        void PushAllUpstreamRemote();
    }
}
