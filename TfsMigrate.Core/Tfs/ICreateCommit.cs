using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMigrate.Core.CommitTree;

namespace TfsMigrate.Core.Tfs
{
    public interface ICreateCommitTree
    {
        CommitNode CreateCommitTree(Changeset changeset, 
            Dictionary<string, Tuple<string, CommitNode>> branches);
    }
}
