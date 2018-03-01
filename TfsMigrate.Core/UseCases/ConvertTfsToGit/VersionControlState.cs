using System.Collections.Generic;
using TfsMigrate.Core.CommitTree.Branches;

namespace TfsMigrate.Core.UseCases.ConvertTfsToGit
{
    public class VersionControlState
    {

        public Branches Branches { get;}
        public Dictionary<string, IEnumerable<int>> CommitWorkItemAssociations { get; }

        public VersionControlState()
        {
            Branches = new Branches();
            CommitWorkItemAssociations = new Dictionary<string, IEnumerable<int>>();
        }
    }
}