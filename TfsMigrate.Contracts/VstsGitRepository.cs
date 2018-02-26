using System;

namespace TfsMigrate.Contracts
{
    public class VstsGitRepository
    {
        public Uri ProjectCollection { get; set; }

        public string TeamProject { get; set; }

        public string RepositoryName { get; set; }
    }
}
