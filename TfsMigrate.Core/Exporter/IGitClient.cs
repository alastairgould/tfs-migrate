using System.Diagnostics;

namespace TfsMigrate.Core.Exporter
{
    public interface IGitClient
    {
        void Init();
        Process FastImport();
        void AddUpstreamRemote(string url);
        void PushAllUpstreamRemote();
    }
}
