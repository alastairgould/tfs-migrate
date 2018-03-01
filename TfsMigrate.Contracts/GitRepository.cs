using System.Collections.Generic;

namespace TfsMigrate.Contracts
{
    public class GitRepository
    {
        public string Path { get; set; }

        public Dictionary<string, IEnumerable<int>> CommitWorkItemAssociations { get; set; }
    }
}
